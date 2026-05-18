/**
 * EnterpriseCMS Block Editor
 * jQuery UI Sortable block canvas with palette, settings panel, and autosave.
 */
window.CmsEditor = (function ($) {
    'use strict';

    var config = {};
    var blocks = [];
    var isDirty = false;
    var autoSaveInterval = null;

    function init(options) {
        config = options;
        try { blocks = JSON.parse(config.blocks || '[]'); } catch (e) { blocks = []; }

        renderAllBlocks();
        bindPalette();
        initSortable();
        startAutosave();
        bindBeforeUnload();
    }

    function renderAllBlocks() {
        var $canvas = $('#block-canvas');
        $canvas.empty();
        $.each(blocks, function (i, block) {
            $canvas.append(createBlockCard(block));
        });
        serializeBlocks();
    }

    function createBlockCard(block) {
        var $card = $('<div>', {
            'class': 'block-card card mb-2 shadow-sm',
            'data-block-id': block.id,
            'data-block-type': block.type
        });

        var $header = $('<div class="card-header d-flex align-items-center gap-2 py-1 px-2 bg-white">');
        $header.append('<span class="bi bi-grip-vertical text-muted drag-handle" style="cursor:grab"></span>');
        var $label = $('<span class="small fw-semibold text-capitalize flex-grow-1">').text(block.type);
        $header.append($label);
        $header.append(
            $('<button type="button" class="btn btn-sm btn-outline-secondary py-0 px-1 settings-btn"><i class="bi bi-gear"></i></button>')
                .data('block-id', block.id).data('block-type', block.type)
        );
        $header.append(
            $('<button type="button" class="btn btn-sm btn-outline-danger py-0 px-1 delete-block-btn"><i class="bi bi-trash"></i></button>')
                .data('block-id', block.id)
        );
        $card.append($header);

        var $preview = getBlockPreview(block);
        if ($preview) {
            var $body = $('<div class="card-body py-2 px-3 small text-muted">');
            $body.append($preview);
            $card.append($body);
        }

        return $card;
    }

    function getBlockPreview(block) {
        var data = block.data || {};
        var $container = $('<div>');
        switch (block.type) {
            case 'text':
                if (data.content) {
                    var textVal = $('<div>').html(data.content).text().substring(0, 80) + '\u2026';
                    $container.text(textVal);
                } else {
                    $container.append($('<em>').text('Rich text block'));
                }
                break;
            case 'image':
                if (data.src) {
                    var $img = $('<img>').attr('src', data.src).attr('alt', data.alt || '').css('max-height', '60px');
                    $container.append($img);
                } else {
                    $container.append($('<em>').text('Image block'));
                }
                break;
            case 'button':
                if (data.label) {
                    $container.append($('<button class="btn btn-sm btn-primary disabled">').text(data.label));
                } else {
                    $container.append($('<em>').text('Button block'));
                }
                break;
            case 'divider':
                $container.append($('<hr>'));
                break;
            case 'video':
                if (data.url) {
                    $container.append($('<em>').text('Video: ' + data.url));
                } else {
                    $container.append($('<em>').text('Video block'));
                }
                break;
            case 'html':
                if (data.code) {
                    $container.append($('<code>').text(data.code.substring(0, 60) + '\u2026'));
                } else {
                    $container.append($('<em>').text('HTML block'));
                }
                break;
            case 'columns':
                $container.append($('<em>').text('Columns layout: ' + (data.count || 2) + ' cols'));
                break;
            case 'gallery':
                $container.append($('<em>').text('Gallery block'));
                break;
            default:
                return null;
        }
        return $container;
    }

    function bindPalette() {
        $(document).on('click', '.add-block-btn', function () {
            var blockType = $(this).data('block-type');
            addBlock(blockType);
        });
    }

    function addBlock(blockType) {
        var block = {
            id: generateId(),
            type: blockType,
            order: blocks.length,
            data: {}
        };
        blocks.push(block);
        $('#block-canvas').append(createBlockCard(block));
        serializeBlocks();
        markDirty();
        // Open settings panel for new block
        loadBlockSettings(block.id, block.type);
    }

    function initSortable() {
        $('#block-canvas').sortable({
            handle: '.drag-handle',
            placeholder: 'block-placeholder',
            update: function () {
                reindexBlocks();
                serializeBlocks();
                markDirty();
            }
        });
    }

    function reindexBlocks() {
        var newOrder = [];
        $('#block-canvas .block-card').each(function (i) {
            var id = $(this).data('block-id');
            var existing = blocks.find(function (b) { return b.id === id; });
            if (existing) {
                existing.order = i;
                newOrder.push(existing);
            }
        });
        blocks = newOrder;
    }

    function serializeBlocks() {
        $('#blocksInput').val(JSON.stringify(blocks));
    }

    function markDirty() {
        isDirty = true;
        $('#autoSaveStatus').text('Unsaved changes');
    }

    // Block settings panel
    $(document).on('click', '.settings-btn', function () {
        var blockId = $(this).data('block-id');
        var blockType = $(this).data('block-type');
        loadBlockSettings(blockId, blockType);
    });

    function loadBlockSettings(blockId, blockType) {
        var url = config.blockSettingsUrl + '?blockType=' + encodeURIComponent(blockType) + '&blockId=' + encodeURIComponent(blockId);
        $.get(url, function (html) {
            $('#block-settings-panel').html(html);
            // Initialize TinyMCE if richtext
            if (blockType === 'text' && window.tinymce) {
                tinymce.remove('#richtext-editor-' + blockId);
                tinymce.init({
                    selector: '#richtext-editor-' + blockId,
                    height: 200,
                    menubar: false,
                    plugins: 'lists link',
                    toolbar: 'bold italic | bullist numlist | link',
                    setup: function (ed) {
                        ed.on('change', function () {
                            updateBlockData(blockId, { content: ed.getContent() });
                        });
                    }
                });
            }
        });
    }

    // Update block data from settings panel
    $(document).on('change input', '#block-settings-panel [data-field]', function () {
        var $panel = $('#block-settings-panel');
        var blockId = $panel.find('[data-block-id]').data('block-id');
        if (!blockId) return;
        var field = $(this).data('field');
        var value = $(this).is(':checkbox') ? $(this).prop('checked') : $(this).val();
        var data = {};
        data[field] = value;
        updateBlockData(blockId, data);
    });

    function updateBlockData(blockId, newData) {
        var block = blocks.find(function (b) { return b.id === blockId; });
        if (!block) return;
        block.data = $.extend(block.data || {}, newData);
        // Update preview using safe DOM insertion
        var $card = $('#block-canvas .block-card[data-block-id="' + blockId + '"]');
        var $preview = getBlockPreview(block);
        if ($preview) {
            var $body = $card.find('.card-body');
            if ($body.length) {
                $body.empty().append($preview);
            } else {
                var $newBody = $('<div class="card-body py-2 px-3 small text-muted">');
                $newBody.append($preview);
                $card.append($newBody);
            }
        }
        serializeBlocks();
        markDirty();
    }

    // Delete block
    $(document).on('click', '.delete-block-btn', function () {
        var blockId = $(this).data('block-id');
        blocks = blocks.filter(function (b) { return b.id !== blockId; });
        $('#block-canvas .block-card[data-block-id="' + blockId + '"]').remove();
        serializeBlocks();
        markDirty();
        $('#block-settings-panel').html('<p class="text-muted small">Select a block to edit its settings.</p>');
    });

    function startAutosave() {
        autoSaveInterval = setInterval(function () {
            if (!isDirty) return;
            var formData = new FormData(document.getElementById('contentForm'));
            $.ajax({
                url: config.autoSaveUrl,
                method: 'POST',
                data: $(formData).serialize ? formData : formData,
                processData: false,
                contentType: false,
                headers: { 'RequestVerificationToken': config.csrfToken },
                success: function (data) {
                    if (data.success) {
                        isDirty = false;
                        var $status = $('#autoSaveStatus');
                        $status.text('Auto-saved at ' + new Date(data.savedAt).toLocaleTimeString());
                        setTimeout(function () { $status.text(''); }, 5000);
                    }
                }
            });
        }, 30000);
    }

    function bindBeforeUnload() {
        $(window).on('beforeunload', function () {
            if (isDirty) return 'You have unsaved changes. Are you sure you want to leave?';
        });
        // Clear dirty flag on form submit
        $('#contentForm').on('submit', function () {
            isDirty = false;
        });
    }

    function generateId() {
        return 'block_' + Math.random().toString(36).substr(2, 9);
    }

    return { init: init, addBlock: addBlock, updateBlockData: updateBlockData };

}(jQuery));
