// Admin JS

// Dark mode
(function() {
    const theme = localStorage.getItem('adminTheme') || 'light';
    document.documentElement.setAttribute('data-bs-theme', theme);
})();

document.addEventListener('DOMContentLoaded', function() {
    // Sidebar toggle
    document.getElementById('sidebarToggle')?.addEventListener('click', function() {
        const sidebar = document.getElementById('sidebar');
        if (sidebar) sidebar.classList.toggle('d-none');
    });

    // Dark mode toggle
    const themeToggle = document.getElementById('themeToggle');
    if (themeToggle) {
        themeToggle.addEventListener('click', function() {
            const html = document.documentElement;
            const current = html.getAttribute('data-bs-theme') || 'light';
            const next = current === 'light' ? 'dark' : 'light';
            html.setAttribute('data-bs-theme', next);
            localStorage.setItem('adminTheme', next);
            const icon = themeToggle.querySelector('i');
            if (icon) {
                icon.className = next === 'dark' ? 'bi bi-sun' : 'bi bi-moon';
            }
        });
        // Set initial icon
        const current = document.documentElement.getAttribute('data-bs-theme') || 'light';
        const icon = themeToggle.querySelector('i');
        if (icon) icon.className = current === 'dark' ? 'bi bi-sun' : 'bi bi-moon';
    }
});
