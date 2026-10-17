

(function () {
    'use strict';

    var dropdown = document.getElementById('wardDropdown');
    var btnGo = document.getElementById('btnWardGo');

    if (!dropdown) return;

    // "हेर्नुहोस्" button click — navigate to selected ward
    if (btnGo) {
        btnGo.addEventListener('click', function () {
            navigateToWard();
        });
    }

    // Also navigate on Enter key in dropdown
    dropdown.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            navigateToWard();
        }
    });

    function navigateToWard() {
        var val = dropdown.value;
        if (val) {
            window.location.href = '/Ward?wardNo=' + encodeURIComponent(val);
        } else {
            window.location.href = '/Ward';
        }
    }

    // Ward overview table row click — quick navigation
    var wardRows = document.querySelectorAll('tr[data-ward]');
    wardRows.forEach(function (row) {
        row.addEventListener('click', function (e) {
            // Don't intercept clicks on the button inside the row
            if (e.target.closest('a')) return;
            var wardNo = row.getAttribute('data-ward');
            if (wardNo) {
                window.location.href = '/Ward?wardNo=' + encodeURIComponent(wardNo);
            }
        });

        // Hover effect
        row.addEventListener('mouseenter', function () {
            row.style.backgroundColor = 'var(--accent)';
        });
        row.addEventListener('mouseleave', function () {
            row.style.backgroundColor = '';
        });
    });
})();