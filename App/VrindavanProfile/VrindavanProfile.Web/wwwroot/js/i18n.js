// ═══════════════════════════════════════════════════
// VRINDAVAN MUNICIPALITY — i18n Engine
// Part 15: Client-side Language Toggle (Nepali ↔ English)
// ═══════════════════════════════════════════════════

var VrindavanI18n = (function () {
    var _translations = {};
    var _currentLang = 'ne';
    var _ready = false;

    /**
     * Initialize the i18n system.
     * @param {string} lang - Current language ('ne' or 'en')
     */
    function init(lang) {
        _currentLang = lang || 'ne';
        loadTranslations(_currentLang);
    }

    /**
     * Load translations from the API endpoint.
     */
    function loadTranslations(lang) {
        var xhr = new XMLHttpRequest();
        xhr.open('GET', '/Language/Api?lang=' + lang, true);
        xhr.setRequestHeader('Accept', 'application/json');
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                try {
                    _translations = JSON.parse(xhr.responseText);
                    _ready = true;
                    applyAll();
                } catch (e) {
                    console.error('i18n: Failed to parse translations', e);
                }
            }
        };
        xhr.send();
    }

    /**
     * Get a translation by key.
     * @param {string} key - Dot-separated key (e.g. "nav.home")
     * @returns {string}
     */
    function t(key) {
        if (_translations[key]) return _translations[key];
        return key;
    }

    /**
     * Apply translations to all elements with data-i18n attributes.
     */
    function applyAll() {
        if (!_ready) return;

        // data-i18n="key" → set textContent
        document.querySelectorAll('[data-i18n]').forEach(function (el) {
            var key = el.getAttribute('data-i18n');
            var val = t(key);
            if (val !== key) {
                el.textContent = val;
            }
        });

        // data-i18n-placeholder="key" → set placeholder
        document.querySelectorAll('[data-i18n-placeholder]').forEach(function (el) {
            var key = el.getAttribute('data-i18n-placeholder');
            var val = t(key);
            if (val !== key) {
                el.setAttribute('placeholder', val);
            }
        });

        // data-i18n-title="key" → set title attribute
        document.querySelectorAll('[data-i18n-title]').forEach(function (el) {
            var key = el.getAttribute('data-i18n-title');
            var val = t(key);
            if (val !== key) {
                el.setAttribute('title', val);
            }
        });

        // data-i18n-aria="key" → set aria-label
        document.querySelectorAll('[data-i18n-aria]').forEach(function (el) {
            var key = el.getAttribute('data-i18n-aria');
            var val = t(key);
            if (val !== key) {
                el.setAttribute('aria-label', val);
            }
        });

        // data-i18n-html="key" → set innerHTML (for content with line breaks)
        document.querySelectorAll('[data-i18n-html]').forEach(function (el) {
            var key = el.getAttribute('data-i18n-html');
            var val = t(key);
            if (val !== key) {
                el.innerHTML = val;
            }
        });

        // Update document title
        var docTitle = document.querySelector('title');
        if (docTitle) {
            var titleKey = _currentLang === 'en'
                ? 'app.title'
                : 'app.title';
            var titleVal = t(titleKey);
            if (titleVal !== titleKey) {
                // Preserve the suffix
                var currentText = docTitle.textContent;
                var sep = currentText.indexOf(' - ');
                if (sep > -1) {
                    docTitle.textContent = currentText.substring(0, sep) + ' - ' + titleVal.split(' - ')[0];
                }
            }
        }
    }

    /**
     * Switch language and reload page.
     * @param {string} lang - Target language ('ne' or 'en')
     */
    function switchLanguage(lang) {
        if (lang === _currentLang) return;

        // Build return URL
        var returnUrl = window.location.pathname + window.location.search;

        // Navigate to language switch endpoint
        window.location.href = '/Language/Switch?lang=' + lang + '&returnUrl=' + encodeURIComponent(returnUrl);
    }

    /**
     * Get current language.
     */
    function getCurrentLang() {
        return _currentLang;
    }

    /**
     * Check if translations are loaded.
     */
    function isReady() {
        return _ready;
    }

    /**
     * Get the full translations dictionary.
     */
    function getAll() {
        return _translations;
    }

    // Public API
    return {
        init: init,
        t: t,
        applyAll: applyAll,
        switchLanguage: switchLanguage,
        getCurrentLang: getCurrentLang,
        isReady: isReady,
        getAll: getAll
    };
})();
