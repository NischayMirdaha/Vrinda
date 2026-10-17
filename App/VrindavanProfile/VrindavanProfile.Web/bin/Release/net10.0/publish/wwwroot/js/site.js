// VRINDAVAN MUNICIPALITY — Site JS
// Global utilities

document.addEventListener('DOMContentLoaded', function () {
    console.log('वृन्दावन नगरपालिका डिजिटल प्रोफाइल प्रणाली लोड भयो।');
    initCardEntrance();
});

// 9D: Staggered card entrance using IntersectionObserver
function initCardEntrance() {
    var cards = document.querySelectorAll('.chart-card, .table-card');
    if (!cards.length || !('IntersectionObserver' in window)) {
        // Fallback: just show everything
        cards.forEach(function(c) { c.style.opacity = '1'; });
        return;
    }

    cards.forEach(function(card) {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
    });

    var observer = new IntersectionObserver(function(entries) {
        entries.forEach(function(entry) {
            if (entry.isIntersecting) {
                var delay = Array.from(cards).indexOf(entry.target) * 80;
                setTimeout(function() {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }, Math.min(delay, 600)); // cap delay at 600ms
                observer.unobserve(entry.target);
            }
        });
    }, { threshold: 0.1 });

    cards.forEach(function(card) { observer.observe(card); });
}

// Animate number counters
function animateCounters() {
    const counters = document.querySelectorAll('.counter-value');
    counters.forEach(function(counter) {
        const target = parseInt(counter.getAttribute('data-target'));
        if (isNaN(target) || target === 0) {
            counter.textContent = '0';
            return;
        }
        const duration = 1500;
        const startTime = performance.now();

        function updateCounter(currentTime) {
            const elapsed = currentTime - startTime;
            const progress = Math.min(elapsed / duration, 1);
            // easeOutQuart
            const easeOut = 1 - Math.pow(1 - progress, 4);
            const current = Math.floor(easeOut * target);
            counter.textContent = current.toLocaleString('ne-NP');
            if (progress < 1) {
                requestAnimationFrame(updateCounter);
            } else {
                counter.textContent = target.toLocaleString('ne-NP');
            }
        }
        requestAnimationFrame(updateCounter);
    });
}

// Format number in Nepali locale
function formatNepali(num) {
    if (num === null || num === undefined) return '—';
    return Number(num).toLocaleString('ne-NP');
}

// 9E: Utility — truncate Nepali text to fit, add ellipsis
function truncateNepali(text, maxLen) {
    if (!text) return '';
    if (text.length <= maxLen) return text;
    return text.substring(0, maxLen - 1) + '…';
}

// ═══════════════════════════════════════════════════
// TOC Sidebar Toggle (Part 11C)
// ═══════════════════════════════════════════════════
function toggleTocSection(el) {
    el.closest('.toc-section').classList.toggle('open');
}

// ═══════════════════════════════════════════════════
// Column Visibility Toggle (Part 11D)
// ═══════════════════════════════════════════════════
function toggleColVisibilityPanel() {
    var panel = document.getElementById('colTogglePanel');
    if (panel) panel.classList.toggle('show');
}

function toggleTableColumn(colIndex, checkbox) {
    var table = document.getElementById('mainTable');
    if (!table) return;
    var isVisible = checkbox.checked;
    var cells = table.querySelectorAll('tr > *:nth-child(' + (colIndex + 1) + ')');
    cells.forEach(function(cell) {
        if (isVisible) {
            cell.classList.remove('col-hidden');
        } else {
            cell.classList.add('col-hidden');
        }
    });
}

// Close column toggle panel on outside click
document.addEventListener('click', function (e) {
    var panel = document.getElementById('colTogglePanel');
    var btn = document.querySelector('[onclick="toggleColVisibilityPanel()"]');
    if (panel && !panel.contains(e.target) && btn && !btn.contains(e.target)) {
        panel.classList.remove('show');
    }
});
