/* ═══════════════════════════════════════════════════
   SKYADMIN — admin.js
   Sidebar collapse, dark mode, date, mobile menu
════════════════════════════════════════════════════ */

(function () {
    'use strict';

    const shell = document.getElementById('adminShell');
    const sidebar = document.getElementById('sidebar');
    const collapseBtn = document.getElementById('collapseBtn');
    const mobileBtn = document.getElementById('mobileMenuBtn');
    const themeToggle = document.getElementById('themeToggle');
    const dateEl = document.getElementById('topbarDate');

    /* ── Sidebar collapse (desktop) ── */
    const COLLAPSED_KEY = 'sky_sidebar_collapsed';

    function setCollapsed(val) {
        if (val) {
            shell.classList.add('collapsed');
            localStorage.setItem(COLLAPSED_KEY, '1');
        } else {
            shell.classList.remove('collapsed');
            localStorage.removeItem(COLLAPSED_KEY);
        }
    }

    if (collapseBtn) {
        collapseBtn.addEventListener('click', () => {
            setCollapsed(!shell.classList.contains('collapsed'));
        });
    }

    /* Restore on load */
    if (localStorage.getItem(COLLAPSED_KEY)) {
        setCollapsed(true);
    }

    /* ── Mobile sidebar ── */
    let overlay = document.createElement('div');
    overlay.className = 'sb-overlay';
    document.body.appendChild(overlay);

    function openMobile() {
        shell.classList.add('mobile-open');
        overlay.classList.add('show');
        document.body.style.overflow = 'hidden';
    }
    function closeMobile() {
        shell.classList.remove('mobile-open');
        overlay.classList.remove('show');
        document.body.style.overflow = '';
    }

    if (mobileBtn) {
        mobileBtn.addEventListener('click', () => {
            shell.classList.contains('mobile-open') ? closeMobile() : openMobile();
        });
    }
    overlay.addEventListener('click', closeMobile);

    /* Close mobile sidebar on nav click */
    document.querySelectorAll('.nav-item').forEach(item => {
        item.addEventListener('click', () => {
            if (window.innerWidth <= 768) closeMobile();
        });
    });

    /* ── Dark mode ── */
    const THEME_KEY = 'sky_theme';

    function setTheme(theme) {
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem(THEME_KEY, theme);
    }

    function getPreferredTheme() {
        const saved = localStorage.getItem(THEME_KEY);
        if (saved) return saved;
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }

    setTheme(getPreferredTheme());

    if (themeToggle) {
        themeToggle.addEventListener('click', () => {
            const current = document.documentElement.getAttribute('data-theme');
            setTheme(current === 'dark' ? 'light' : 'dark');
        });
    }

    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
        if (!localStorage.getItem(THEME_KEY)) {
            setTheme(e.matches ? 'dark' : 'light');
        }
    });

    /* ── Date display ── */
    if (dateEl) {
        const now = new Date();
        dateEl.textContent = now.toLocaleDateString('en-US', {
            weekday: 'short',
            month: 'short',
            day: 'numeric',
            year: 'numeric'
        });
    }

    /* ── Tooltip for collapsed sidebar nav items ── */
    function buildTooltips() {
        document.querySelectorAll('.nav-item').forEach(item => {
            const label = item.querySelector('.nav-label');
            if (!label) return;
            item.setAttribute('title', label.textContent.trim());
        });
    }
    buildTooltips();

    /* ── Active page highlight via data-page ── */
    const page = document.body.getAttribute('data-page');
    if (page) {
        document.querySelectorAll('.nav-item').forEach(item => {
            const href = item.getAttribute('href') || '';
            if (href.toLowerCase().includes(page.toLowerCase())) {
                item.classList.add('active');
            }
        });
    }

    /* ── Resize: close mobile on desktop resize ── */
    window.addEventListener('resize', () => {
        if (window.innerWidth > 768) closeMobile();
    });

})();