/* ============================================================
   AeroFly – Premium Airline Reservation System
   Interactive UI Components
   ============================================================ */

(function() {
    "use strict";

    // ---------- Sticky Navbar ----------
    const header = document.querySelector('.site-header');
    
    function handleScroll() {
        if (window.scrollY > 30) {
            header.classList.add('scrolled');
        } else {
            header.classList.remove('scrolled');
        }
    }
    
    window.addEventListener('scroll', handleScroll);
    handleScroll();

    // ---------- Flight Type Tabs ----------
    const typeBtns = document.querySelectorAll('.type-btn');
    
    typeBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            typeBtns.forEach(b => b.classList.remove('active'));
            this.classList.add('active');
            console.log(`[AeroFly] Flight type: ${this.textContent}`);
        });
    });

    // ---------- Swap Airports ----------
    const swapBtns = document.querySelectorAll('.swap-btn');
    
    swapBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            const form = this.closest('form');
            if (form) {
                const inputs = form.querySelectorAll('.search-input');
                if (inputs.length >= 2) {
                    const fromVal = inputs[0].value;
                    const toVal = inputs[1].value;
                    inputs[0].value = toVal;
                    inputs[1].value = fromVal;
                    
                    // Visual feedback
                    this.style.transform = 'rotate(180deg)';
                    setTimeout(() => {
                        this.style.transform = '';
                    }, 300);
                }
            }
        });
    });

    // ---------- Search Handler ----------
    const searchBtns = document.querySelectorAll('.btn-search-primary');
    
    searchBtns.forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            const form = this.closest('form');
            if (!form) return;
            
            const inputs = form.querySelectorAll('.search-input');
            const from = inputs[0]?.value || '';
            const to = inputs[1]?.value || '';
            const date = inputs[2]?.value || '';
            
            if (!from || !to) {
                alert('Please enter both departure and destination cities.');
                return;
            }
            
            console.log(`[AeroFly] Searching: ${from} → ${to} on ${date}`);
            
            // Visual feedback
            const originalText = this.innerHTML;
            this.innerHTML = '<i class="bi bi-search-heart"></i> Searching...';
            setTimeout(() => {
                this.innerHTML = originalText;
                // Redirect logic mockup:
                // window.location.href = 'search-results.html';
            }, 800);
        });
    });

    // ---------- View Flight Buttons ----------
    const viewBtns = document.querySelectorAll('.btn-view');
    
    viewBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            const card = this.closest('.destination-card');
            const destination = card?.querySelector('h3')?.textContent || 'selected destination';
            console.log(`[AeroFly] Viewing flights to ${destination}`);
        });
    });

    // ---------- Learn More Button ----------
    const learnBtn = document.querySelector('.btn-learn');
    
    if (learnBtn) {
        learnBtn.addEventListener('click', function() {
            console.log('[AeroFly] Exploring premium benefits');
        });
    }

    // ---------- Newsletter Subscription ----------
    const newsletterBtn = document.querySelector('.newsletter-form button');
    const newsletterInput = document.querySelector('.newsletter-form input');
    
    if (newsletterBtn) {
        newsletterBtn.addEventListener('click', function() {
            const email = newsletterInput?.value.trim();
            if (email && email.includes('@')) {
                alert(`Thank you for subscribing! Premium deals will be sent to ${email}`);
                newsletterInput.value = '';
            } else if (email) {
                alert('Please enter a valid email address.');
            }
        });
    }

    // ---------- Smooth Scroll for Anchor Links ----------
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            const href = this.getAttribute('href');
            if (href && href !== '#' && href !== '#') {
                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    target.scrollIntoView({ behavior: 'smooth' });
                }
            }
        });
    });

    // ---------- Intersection Observer for Animations ----------
    const animatedElements = document.querySelectorAll('.destination-card, .feature-item');
    
    if ('IntersectionObserver' in window) {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.1 });
        
        animatedElements.forEach(el => {
            el.style.opacity = '0';
            el.style.transform = 'translateY(25px)';
            el.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
            observer.observe(el);
        });
    } else {
        animatedElements.forEach(el => {
            el.style.opacity = '1';
            el.style.transform = 'translateY(0)';
        });
    }

    // ---------- Console Ready ----------
    console.log('✈️ AeroFly UI Ready — Premium Airline Experience');
})();