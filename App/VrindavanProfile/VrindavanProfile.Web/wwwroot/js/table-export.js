// ═══════════════════════════════════════════════════════════════
// VRINDAVAN MUNICIPALITY — Export Utilities (Part 10C)
// Handles Print, PDF export for tables, dashboard, and maps
// ═══════════════════════════════════════════════════════════════

(function () {
    'use strict';

    // ─── Library Loader Cache ───────────────────────────
    var _loaded = {};

    function loadScript(url) {
        return new Promise(function (resolve, reject) {
            if (_loaded[url]) { resolve(); return; }
            var s = document.createElement('script');
            s.src = url;
            s.onload = function () { _loaded[url] = true; resolve(); };
            s.onerror = function () { reject(new Error('Failed to load: ' + url)); };
            document.head.appendChild(s);
        });
    }

    function loadCss(url) {
        return new Promise(function (resolve) {
            if (_loaded[url]) { resolve(); return; }
            var link = document.createElement('link');
            link.rel = 'stylesheet';
            link.href = url;
            link.onload = function () { _loaded[url] = true; resolve(); };
            document.head.appendChild(link);
        });
    }

    function loadLibraries() {
        return loadScript('https://cdn.jsdelivr.net/npm/html2canvas@1.4.1/dist/html2canvas.min.js')
            .then(function () {
                return loadScript('https://cdn.jsdelivr.net/npm/jspdf@2.5.2/dist/jspdf.umd.min.js');
            });
    }

    // ─── Export Overlay (Progress Dialog) ──────────────
    function showExportOverlay(title, message) {
        var existing = document.getElementById('exportOverlay');
        if (existing) existing.remove();

        var overlay = document.createElement('div');
        overlay.id = 'exportOverlay';
        overlay.className = 'export-overlay active';
        overlay.innerHTML =
            '<div class="export-dialog">' +
            '<div class="spinner-border" role="status"><span class="visually-hidden">...</span></div>' +
            '<h6>' + (title || 'PDF उत्पादन गर्दै') + '</h6>' +
            '<p>' + (message || 'कृपया पर्खनुहोस्...') + '</p>' +
            '<div class="progress"><div class="progress-bar progress-bar-striped progress-bar-animated" style="width:100%"></div></div>' +
            '</div>';
        document.body.appendChild(overlay);
        return overlay;
    }

    function hideExportOverlay() {
        var overlay = document.getElementById('exportOverlay');
        if (overlay) {
            overlay.style.opacity = '0';
            overlay.style.transition = 'opacity 0.3s';
            setTimeout(function () { overlay.remove(); }, 300);
        }
    }

    // ─── Filename Generator ────────────────────────────
    function generateFilename(prefix) {
        var titleEl = document.querySelector('.print-title, h4.fw-bold, .section-title, h1, h2');
        var base = prefix || 'vrindavan';
        if (titleEl) {
            var text = titleEl.textContent.trim()
                .replace(/[^\u0900-\u097F\u0000-\u007Fa-zA-Z0-9\s\-]/g, '')
                .replace(/\s+/g, '-')
                .substring(0, 50);
            if (text.length > 2) base += '-' + text;
        }
        var now = new Date();
        base += '-' + now.getFullYear() +
            String(now.getMonth() + 1).padStart(2, '0') +
            String(now.getDate()).padStart(2, '0');
        return base + '.pdf';
    }

    // ─── Capture Area to PDF ───────────────────────────
    function captureToPDF(elementId, filename, options) {
        options = options || {};
        var el = document.getElementById(elementId);
        if (!el) {
            alert('प्रिन्ट क्षेत्र भेटिएन।');
            return Promise.reject('Element not found: ' + elementId);
        }

        showExportOverlay('PDF उत्पादन गर्दै', 'कृपया पर्खनुहोस्, कागजात तयार हुँदैछ...');

        return loadLibraries().then(function () {
            // Force charts/canvases to render before capture
            var canvases = el.querySelectorAll('canvas');
            canvases.forEach(function (c) {
                c.style.display = 'block';
            });

            return html2canvas(el, {
                scale: 2,
                useCORS: true,
                allowTaint: true,
                backgroundColor: '#FFFFFF',
                logging: false,
                scrollX: 0,
                scrollY: -window.scrollY,
                windowWidth: el.scrollWidth,
                windowHeight: el.scrollHeight,
                onclone: function (doc) {
                    // Ensure cloned element has white background
                    var clonedEl = doc.getElementById(elementId);
                    if (clonedEl) {
                        clonedEl.style.background = '#fff';
                        clonedEl.style.padding = '10px';
                        // Remove skeletons
                        doc.querySelectorAll('.chart-skeleton').forEach(function (s) { s.style.display = 'none'; });
                        doc.querySelectorAll('.export-btn-group').forEach(function (b) { b.style.display = 'none'; });
                    }
                    // Expand all collapsed sections
                    doc.querySelectorAll('.collapse').forEach(function (c) {
                        c.style.display = 'block';
                        c.style.height = 'auto';
                    });
                }
            });
        }).then(function (canvas) {
            var jsPDF = window.jspdf.jsPDF;
            var imgWidth = 210;  // A4 width in mm
            var pageHeight = 297; // A4 height in mm
            var imgHeight = canvas.height * imgWidth / canvas.width;
            var heightLeft = imgHeight;
            var position = 0;

            var doc = new jsPDF('p', 'mm', 'a4');

            // No manual green header is drawn since the HTML content captured contains the necessary headers
            var topMargin = 10;
            var bottomMargin = 15;
            var contentHeight = pageHeight - topMargin - bottomMargin; // 272 mm of content height per page

            // Add content image for the first page
            doc.addImage(canvas.toDataURL('image/jpeg', 0.92), 'JPEG', 0, topMargin, imgWidth, imgHeight);

            heightLeft -= contentHeight;

            while (heightLeft > 0) {
                doc.addPage();
                position = heightLeft - imgHeight;

                doc.addImage(canvas.toDataURL('image/jpeg', 0.92), 'JPEG', 0, position + topMargin, imgWidth, imgHeight);
                heightLeft -= contentHeight;
            }

            // Add footer on all pages
            var pageCount = doc.internal.getNumberOfPages();
            for (var i = 1; i <= pageCount; i++) {
                doc.setPage(i);
                doc.setFontSize(7);
                doc.setTextColor(150, 150, 150);
                var dateStr = new Date().toLocaleDateString('en-US') + ' ' + new Date().toLocaleTimeString('en-US');
                doc.text(
                    'Print Date: ' + dateStr + '  |  Vrindavan Municipality Digital Profile  |  Page ' + i + ' / ' + pageCount,
                    105, 287, { align: 'center' }
                );
            }

            doc.save(filename || generateFilename('export'));

            hideExportOverlay();
            return doc;
        }).catch(function (err) {
            console.error('PDF generation failed:', err);
            hideExportOverlay();
            alert('PDF उत्पादन गर्न सकिएन। कृपया ब्राउजरको प्रिन्ट (Ctrl+P) प्रयोग गर्नुहोस्।\n\nत्रुटि: ' + (err.message || err));
        });
    }

    // ─── Table PDF Export ──────────────────────────────
    // Exposed globally for backward compatibility
    window.exportTablePDF = function () {
        return captureToPDF('printArea', generateFilename('table'));
    };

    // ─── Dashboard PDF Export ──────────────────────────
    window.exportDashboardPDF = function () {
        // Create a temporary wrapper for the entire dashboard content
        var mainContent = document.querySelector('main');
        if (!mainContent) {
            alert('ड्यासबोर्ड सामग्री भेटिएन।');
            return;
        }

        // Temporarily add a print header inside main for capture
        var headerDiv = document.createElement('div');
        headerDiv.className = 'print-only';
        headerDiv.id = 'tempPrintHeader';
        headerDiv.innerHTML =
            '<div style="text-align:center;border-bottom:2px solid #2D6A4F;padding-bottom:8px;margin-bottom:12px">' +
            '<h2 style="color:#2D6A4F;margin:0">वृन्दावन नगरपालिका — डिजिटल प्रोफाइल</h2>' +
            '<p style="color:#6C757D;margin:2px 0;font-size:0.85rem">रुपन्देही जिल्ला, लुम्बिनी प्रदेश, नेपाल — ड्यासबोर्ड सारांश</p>' +
            '</div>';
        mainContent.insertBefore(headerDiv, mainContent.firstChild);

        // Set ID temporarily if not already set
        var originalId = mainContent.id;
        mainContent.id = 'dashboardPrintArea';

        captureToPDF('dashboardPrintArea', generateFilename('dashboard')).then(function () {
            // Restore
            mainContent.id = originalId;
            var tempHeader = document.getElementById('tempPrintHeader');
            if (tempHeader) tempHeader.remove();
        }).catch(function () {
            mainContent.id = originalId;
            var tempHeader = document.getElementById('tempPrintHeader');
            if (tempHeader) tempHeader.remove();
        });
    };

    // ─── Ward Profile PDF Export ───────────────────────
    window.exportWardPDF = function () {
        var mainContent = document.querySelector('main');
        if (!mainContent) {
            alert('वडा प्रोफाइल सामग्री भेटिएन।');
            return;
        }

        var headerDiv = document.createElement('div');
        headerDiv.className = 'print-only';
        headerDiv.id = 'tempWardPrintHeader';
        headerDiv.innerHTML =
            '<div style="text-align:center;border-bottom:2px solid #2D6A4F;padding-bottom:8px;margin-bottom:12px">' +
            '<h2 style="color:#2D6A4F;margin:0">वृन्दावन नगरपालिका — वडा प्रोफाइल</h2>' +
            '<p style="color:#6C757D;margin:2px 0;font-size:0.85rem">रुपन्देही जिल्ला, लुम्बिनी प्रदेश, नेपाल</p>' +
            '</div>';
        mainContent.insertBefore(headerDiv, mainContent.firstChild);

        var originalId = mainContent.id;
        mainContent.id = 'wardPrintArea';

        captureToPDF('wardPrintArea', generateFilename('ward-profile')).then(function () {
            mainContent.id = originalId;
            var h = document.getElementById('tempWardPrintHeader');
            if (h) h.remove();
        }).catch(function () {
            mainContent.id = originalId;
            var h = document.getElementById('tempWardPrintHeader');
            if (h) h.remove();
        });
    };

    // ─── Map PDF Export ────────────────────────────────
    window.exportMapPDF = function () {
        var mapEl = document.getElementById('mapContainer');
        if (!mapEl) {
            alert('नक्सा भेटिएन।');
            return;
        }

        showExportOverlay('नक्सा PDF उत्पादन गर्दै', 'कृपया पर्खनुहोस्...');

        loadLibraries().then(function () {
            return html2canvas(mapEl, {
                scale: 2,
                useCORS: true,
                allowTaint: true,
                backgroundColor: '#E8E8E8',
                logging: false
            });
        }).then(function (canvas) {
            var jsPDF = window.jspdf.jsPDF;
            var doc = new jsPDF('l', 'mm', 'a4'); // landscape for maps

            // Map image (no manual green header is drawn to avoid garbled characters)
            var imgWidth = 297;
            var imgHeight = canvas.height * imgWidth / canvas.width;
            var topOffset = 10;
            var maxH = 210 - topOffset - 15;
            if (imgHeight > maxH) imgHeight = maxH;

            doc.addImage(canvas.toDataURL('image/jpeg', 0.92), 'JPEG', 0, topOffset, imgWidth, imgHeight);

            // Footer
            doc.setFontSize(7);
            doc.setTextColor(150);
            doc.text('Vrindavan Municipality Digital Profile — Map Export', 148.5, 202, { align: 'center' });

            doc.save(generateFilename('map'));
            hideExportOverlay();
        }).catch(function (err) {
            console.error('Map PDF failed:', err);
            hideExportOverlay();
            alert('नक्सा PDF उत्पादन गर्न सकिएन।');
        });
    };

    // ─── Browser Print ────────────────────────────────
    window.printPage = function () {
        // Add a temporary print title
        var printTitle = document.querySelector('.print-title');
        if (!printTitle) {
            var main = document.querySelector('main');
            if (main) {
                var t = document.createElement('div');
                t.className = 'print-only';
                t.style.cssText = 'text-align:center;border-bottom:2px solid #2D6A4F;padding-bottom:6px;margin-bottom:10px;';
                var h2 = document.createElement('h2');
                h2.style.cssText = 'color:#2D6A4F;margin:0;font-size:16px;';
                h2.textContent = 'वृन्दावन नगरपालिका — डिजिटल प्रोफाइल';
                t.appendChild(h2);
                var sub = document.createElement('p');
                sub.style.cssText = 'color:#6C757D;font-size:10px;margin:2px 0;';
                sub.textContent = 'रुपन्देही जिल्ला, लुम्बिनी प्रदेश, नेपाल';
                t.appendChild(sub);
                main.insertBefore(t, main.firstChild);
            }
        }
        window.print();
    };

    // ─── Table CSV Export ──────────────────────────────
    window.exportTableCSV = function () {
        var table = document.querySelector('.data-table, #mainTable, table.table');
        if (!table) {
            alert('तालिका भेटिएन।');
            return;
        }

        var rows = table.querySelectorAll('tr');
        var csv = [];
        rows.forEach(function (row) {
            var cols = row.querySelectorAll('td, th');
            var rowData = [];
            cols.forEach(function (col) {
                // Escape quotes and wrap in quotes
                var text = col.textContent.trim().replace(/"/g, '""');
                rowData.push('"' + text + '"');
            });
            csv.push(rowData.join(','));
        });

        var csvContent = '\uFEFF' + csv.join('\n'); // BOM for UTF-8
        var blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        var link = document.createElement('a');
        link.href = URL.createObjectURL(blob);

        var titleEl = document.querySelector('h4.fw-bold, .section-title');
        var name = 'vrindavan-table';
        if (titleEl) name += '-' + titleEl.textContent.trim().replace(/\s+/g, '-').substring(0, 30);
        link.download = name + '.csv';
        link.click();
        URL.revokeObjectURL(link.href);
    };

    // ─── Keyboard Shortcuts ───────────────────────────
    document.addEventListener('keydown', function (e) {
        // Ctrl+P — override with our print
        if (e.ctrlKey && e.key === 'p') {
            e.preventDefault();
            window.printPage();
        }
    });

})();