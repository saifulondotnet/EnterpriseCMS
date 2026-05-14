// Admin JS
document.getElementById('sidebarToggle')?.addEventListener('click', function() {
    const sidebar = document.getElementById('sidebar');
    if (sidebar) sidebar.classList.toggle('d-none');
});
