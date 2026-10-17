// wwwroot/js/charts.js
// Chart.js rendering helpers for Vrindavan Municipality Dashboard
// Part 9A: Enhanced with animations, custom tooltips, responsive options

const PALETTE = [
    '#003B6F', '#0066B3', '#1A6FB5', '#0EA5E9', '#87CEEB',
    '#DC143C', '#E8384F', '#C5A55A', '#0D9488', '#002147',
    '#B01030', '#3B82F6', '#D4A017', '#6B7280', '#94A3B8'
];

const PALETTE_HALF_OPACITY = PALETTE.map(c => c + '80');

function nepaliNum(n) {
    if (n === null || n === undefined) return '\u2014';
    return Number(n).toLocaleString('ne-NP');
}

function formatNum(n) {
    if (n === null || n === undefined) return '\u2014';
    if (n >= 1000000) return (n / 1000000).toFixed(1) + 'M';
    if (n >= 1000) return (n / 1000).toFixed(1) + 'K';
    return n.toString();
}

// Register datalabels plugin globally, disable by default
if (typeof ChartDataLabels !== 'undefined') {
    Chart.register(ChartDataLabels);
    Chart.defaults.plugins.datalabels.display = false;
}

// 9E: Chart.js global defaults for Nepali/Devanagari text
// Guard: ensure Chart.defaults.font exists before setting properties
Chart.defaults.font = Chart.defaults.font || {};
Chart.defaults.font.family = "'Mukta', 'Noto Sans Devanagari', 'Kalimati', 'Segoe UI', sans-serif";
Chart.defaults.font.size = 11;
Chart.defaults.plugins.tooltip.titleFont = Chart.defaults.plugins.tooltip.titleFont || {};
Chart.defaults.plugins.tooltip.titleFont.family = "'Mukta', 'Noto Sans Devanagari', 'Segoe UI', sans-serif";
Chart.defaults.plugins.tooltip.bodyFont = Chart.defaults.plugins.tooltip.bodyFont || {};
Chart.defaults.plugins.tooltip.bodyFont.family = "'Mukta', 'Noto Sans Devanagari', 'Segoe UI', sans-serif";
Chart.defaults.plugins.legend.labels = Chart.defaults.plugins.legend.labels || {};
Chart.defaults.plugins.legend.labels.font = Chart.defaults.plugins.legend.labels.font || {};
Chart.defaults.plugins.legend.labels.font.family = "'Mukta', 'Noto Sans Devanagari', 'Segoe UI', sans-serif";
// 9E: Allow wrapping for long Nepali labels
Chart.defaults.plugins.legend.labels.boxWidth = 12;
Chart.defaults.plugins.legend.labels.padding = 10;
// 9E: Improve animation
Chart.defaults.animation.duration = 800;
Chart.defaults.animation.easing = 'easeOutQuart';
// 9E: Responsive by default
Chart.defaults.responsive = true;
Chart.defaults.maintainAspectRatio = false;

// ═══════════════════════════════════════════════════
// SHARED OPTIONS
// ═══════════════════════════════════════════════════

const SHARED_ANIMATION = {
    duration: 800,
    easing: 'easeOutQuart'
};

const SHARED_TOOLTIP = {
    backgroundColor: 'rgba(27,27,27,0.92)',
    titleFont: { size: 13, weight: 'bold', family: "'Mukta', 'Noto Sans Devanagari', sans-serif" },
    bodyFont: { size: 12, family: "'Mukta', 'Noto Sans Devanagari', sans-serif" },
    footerFont: { family: "'Mukta', 'Noto Sans Devanagari', sans-serif" },
    padding: 12,
    cornerRadius: 8,
    displayColors: true,
    boxPadding: 4,
    borderColor: 'rgba(255,255,255,0.1)',
    borderWidth: 1,
    bodySpacing: 4,
    titleSpacing: 2
};

const RESPONSIVE_OPTIONS = {
    responsive: true,
    maintainAspectRatio: false,
    animation: SHARED_ANIMATION,
    layout: { padding: { top: 4, bottom: 4 } }
};

function percentTooltip(context) {
    var total = context.dataset.data.reduce(function (a, b) { return a + b; }, 0);
    var pct = ((context.parsed / total) * 100).toFixed(1);
    return context.label + ': ' + nepaliNum(context.parsed) + ' (' + pct + '%)';
}

function valueTooltip(context) {
    return (context.dataset.label || context.label) + ': ' + nepaliNum(context.parsed.y || context.parsed);
}

function groupedTooltip(context) {
    return context.dataset.label + ': ' + nepaliNum(context.parsed.y);
}

function horizontalTooltip(context) {
    return context.dataset.label + ': ' + nepaliNum(Math.abs(context.parsed.x));
}

function stackedTooltip(context) {
    var dsIndex = context.datasetIndex;
    var raw = context.dataset.data[context.dataIndex];
    return context.dataset.label + ': ' + nepaliNum(raw);
}

// ═══════════════════════════════════════════════════
// MAIN RENDER DISPATCHER
// ═══════════════════════════════════════════════════

function renderAllCharts(charts) {
    if (!charts || charts.length === 0) return;
    charts.forEach(function (chartData) {
        var canvas = document.getElementById('chart_' + chartData.id);
        if (!canvas) return;
        var ctx = canvas.getContext('2d');
        var colors = PALETTE.slice(0, (chartData.items || []).length);
        switch (chartData.chartType) {
            case 'doughnut': renderDoughnut(ctx, chartData, colors); break;
            case 'bar': renderBar(ctx, chartData, colors); break;
            case 'pie': renderPie(ctx, chartData, colors); break;
            case 'horizontalBar': renderHorizontalBar(ctx, chartData, colors); break;
            case 'line': renderLine(ctx, chartData, colors); break;
            case 'stackedBar': renderStackedBar(ctx, chartData, colors); break;
            case 'groupedBar': renderGroupedBar(ctx, chartData, colors); break;
            case 'agePyramid': renderAgePyramid(ctx, chartData, colors); break;
            default: renderDoughnut(ctx, chartData, colors);
        }
    });
}

// ═══════════════════════════════════════════════════
// DOUGHNUT
// ═══════════════════════════════════════════════════

function renderDoughnut(ctx, data, colors) {
    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: data.items.map(function (i) { return i.label; }),
            datasets: [{
                data: data.items.map(function (i) { return i.value; }),
                backgroundColor: colors,
                borderColor: '#FFFFFF',
                borderWidth: 2,
                hoverBorderWidth: 3,
                hoverOffset: 10
            }]
        },
        options: Object.assign({}, RESPONSIVE_OPTIONS, {
            cutout: '55%',
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: { padding: 10, usePointStyle: true, pointStyleWidth: 10, font: { size: 11 } }
                },
                tooltip: Object.assign({}, SHARED_TOOLTIP, {
                    callbacks: { label: percentTooltip }
                }),
                datalabels: {
                    display: function (context) {
                        var total = context.dataset.data.reduce(function (a, b) { return a + b; }, 0);
                        return (context.dataset.data[context.dataIndex] / total) > 0.05;
                    },
                    color: '#FFFFFF',
                    font: { weight: 'bold', size: 11 },
                    formatter: function (value, context) {
                        var total = context.dataset.data.reduce(function (a, b) { return a + b; }, 0);
                        return ((value / total) * 100).toFixed(0) + '%';
                    }
                }
            }
        })
    });
}

// ═══════════════════════════════════════════════════
// PIE
// ═══════════════════════════════════════════════════

function renderPie(ctx, data, colors) {
    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: data.items.map(function (i) { return i.label; }),
            datasets: [{
                data: data.items.map(function (i) { return i.value; }),
                backgroundColor: colors,
                borderColor: '#FFFFFF',
                borderWidth: 2,
                hoverOffset: 10
            }]
        },
        options: Object.assign({}, RESPONSIVE_OPTIONS, {
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: { padding: 10, usePointStyle: true, font: { size: 11 } }
                },
                tooltip: Object.assign({}, SHARED_TOOLTIP, {
                    callbacks: { label: percentTooltip }
                }),
                datalabels: {
                    display: function (context) {
                        var total = context.dataset.data.reduce(function (a, b) { return a + b; }, 0);
                        return (context.dataset.data[context.dataIndex] / total) > 0.05;
                    },
                    color: '#FFFFFF',
                    font: { weight: 'bold', size: 11 },
                    formatter: function (value, context) {
                        var total = context.dataset.data.reduce(function (a, b) { return a + b; }, 0);
                        return ((value / total) * 100).toFixed(0) + '%';
                    }
                }
            }
        })
    });
}

// ═══════════════════════════════════════════════════
// BAR (auto-detects gender grouping)
// ═══════════════════════════════════════════════════

function renderBar(ctx, data, colors) {
    var hasGender = data.items.some(function (i) { return i.maleCount !== null && i.maleCount !== undefined; });
    if (hasGender) {
        renderGroupedBar(ctx, data, colors);
    } else {
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.items.map(function (i) { return i.label; }),
                datasets: [{
                    label: data.title,
                    data: data.items.map(function (i) { return i.value; }),
                    backgroundColor: colors,
                    borderRadius: 4,
                    borderSkipped: false,
                    maxBarThickness: 40
                }]
            },
            options: getBarOptions(false)
        });
    }
}

function getBarOptions(isGrouped) {
    return Object.assign({}, RESPONSIVE_OPTIONS, {
        indexAxis: 'x',
        plugins: {
            legend: { display: isGrouped, position: 'top', labels: { usePointStyle: true, padding: 12, font: { size: 11 } } },
            tooltip: Object.assign({}, SHARED_TOOLTIP, {
                callbacks: { label: isGrouped ? groupedTooltip : valueTooltip }
            }),
            datalabels: { display: false }
        },
        scales: {
            x: {
                ticks: { font: { size: 10 }, maxRotation: 45, minRotation: 0 },
                grid: { display: false }
            },
            y: {
                beginAtZero: true,
                ticks: {
                    font: { size: 10 },
                    callback: function (value) { return formatNum(value); }
                },
                grid: { color: 'rgba(0,0,0,0.05)' }
            }
        }
    });
}

// ═══════════════════════════════════════════════════
// GROUPED BAR (male/female side by side)
// ═══════════════════════════════════════════════════

function renderGroupedBar(ctx, data, colors) {
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.items.map(function (i) { return i.label; }),
            datasets: [
                {
                    label: '\u092A\u0941\u0930\u0941\u0937',
                    data: data.items.map(function (i) { return i.maleCount || 0; }),
                    backgroundColor: '#003B6F',
                    borderRadius: 4,
                    borderSkipped: false,
                    maxBarThickness: 30
                },
                {
                    label: '\u092E\u0939\u093F\u0932\u093E',
                    data: data.items.map(function (i) { return i.femaleCount || 0; }),
                    backgroundColor: '#DC143C',
                    borderRadius: 4,
                    borderSkipped: false,
                    maxBarThickness: 30
                }
            ]
        },
        options: getBarOptions(true)
    });
}

// ═══════════════════════════════════════════════════
// HORIZONTAL BAR (auto-detects gender → age pyramid)
// ═══════════════════════════════════════════════════

function renderHorizontalBar(ctx, data, colors) {
    var hasGender = data.items.some(function (i) { return i.maleCount !== null && i.maleCount !== undefined; });

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.items.map(function (i) { return i.label; }),
            datasets: hasGender
                ? [
                    {
                        label: '\u092A\u0941\u0930\u0941\u0937',
                        data: data.items.map(function (i) { return -(i.maleCount || 0); }),
                        backgroundColor: '#003B6F',
                        borderRadius: { topLeft: 4, bottomLeft: 4 },
                        borderSkipped: false,
                        maxBarThickness: 20
                    },
                    {
                        label: '\u092E\u0939\u093F\u0932\u093E',
                        data: data.items.map(function (i) { return i.femaleCount || 0; }),
                        backgroundColor: '#DC143C',
                        borderRadius: { topRight: 4, bottomRight: 4 },
                        borderSkipped: false,
                        maxBarThickness: 20
                    }
                ]
                : [{
                    label: data.title,
                    data: data.items.map(function (i) { return i.value; }),
                    backgroundColor: colors,
                    borderRadius: { topRight: 4, bottomRight: 4 },
                    borderSkipped: false,
                    maxBarThickness: 20
                }]
        },
        options: Object.assign({}, RESPONSIVE_OPTIONS, {
            indexAxis: 'y',
            plugins: {
                legend: {
                    display: hasGender,
                    position: 'top',
                    labels: { usePointStyle: true, padding: 12, font: { size: 11 } }
                },
                tooltip: Object.assign({}, SHARED_TOOLTIP, {
                    callbacks: { label: horizontalTooltip }
                }),
                datalabels: { display: false }
            },
            scales: {
                x: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (v) { return nepaliNum(Math.abs(v)); },
                        font: { size: 10 }
                    },
                    grid: { color: 'rgba(0,0,0,0.05)' }
                },
                y: {
                    grid: { display: false },
                    ticks: { font: { size: 10 } }
                }
            }
        })
    });
}

// ═══════════════════════════════════════════════════
// AGE PYRAMID (special horizontal bar)
// ═══════════════════════════════════════════════════

function renderAgePyramid(ctx, data, colors) {
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.items.map(function (i) { return i.label; }),
            datasets: [
                {
                    label: '\u092A\u0941\u0930\u0941\u0937',
                    data: data.items.map(function (i) { return -(i.maleCount || 0); }),
                    backgroundColor: '#003B6F',
                    borderRadius: { topLeft: 3, bottomLeft: 3 },
                    borderSkipped: false,
                    maxBarThickness: 18
                },
                {
                    label: '\u092E\u0939\u093F\u0932\u093E',
                    data: data.items.map(function (i) { return i.femaleCount || 0; }),
                    backgroundColor: '#DC143C',
                    borderRadius: { topRight: 3, bottomRight: 3 },
                    borderSkipped: false,
                    maxBarThickness: 18
                }
            ]
        },
        options: Object.assign({}, RESPONSIVE_OPTIONS, {
            indexAxis: 'y',
            plugins: {
                legend: {
                    display: true,
                    position: 'top',
                    labels: { usePointStyle: true, padding: 12, font: { size: 11 } }
                },
                tooltip: Object.assign({}, SHARED_TOOLTIP, {
                    callbacks: { label: horizontalTooltip }
                }),
                datalabels: { display: false },
                title: {
                    display: true,
                    text: data.title || '\u0909\u092E\u0947\u0930 \u092A\u093F\u0930\u092E\u093F\u0921',
                    font: { size: 12, weight: 'bold' },
                    color: '#1B1B1B',
                    padding: { bottom: 4 }
                }
            },
            scales: {
                x: {
                    stacked: false,
                    ticks: {
                        callback: function (v) { return nepaliNum(Math.abs(v)); },
                        font: { size: 10 }
                    },
                    grid: { color: 'rgba(0,0,0,0.05)' }
                },
                y: {
                    stacked: false,
                    grid: { display: false },
                    ticks: { font: { size: 9 }, mirror: false }
                }
            }
        })
    });
}

// ═══════════════════════════════════════════════════
// STACKED BAR (multi-dataset stacked)
// ═══════════════════════════════════════════════════

function renderStackedBar(ctx, data, colors) {
    // data.datasets is expected: [{ label, values }]
    var labels = data.items.map(function (i) { return i.label; });
    var datasets = (data.datasets || []).map(function (ds, idx) {
        var c = PALETTE[idx % PALETTE.length];
        return {
            label: ds.label,
            data: ds.values,
            backgroundColor: c,
            borderRadius: idx === (data.datasets.length - 1) ? { topLeft: 4, topRight: 4 } : 0,
            borderSkipped: false,
            maxBarThickness: 50
        };
    });

    new Chart(ctx, {
        type: 'bar',
        data: { labels: labels, datasets: datasets },
        options: Object.assign({}, RESPONSIVE_OPTIONS, {
            plugins: {
                legend: {
                    display: true,
                    position: 'top',
                    labels: { usePointStyle: true, padding: 10, font: { size: 10 } }
                },
                tooltip: Object.assign({}, SHARED_TOOLTIP, {
                    mode: 'index',
                    intersect: false,
                    callbacks: { label: stackedTooltip }
                }),
                datalabels: { display: false }
            },
            scales: {
                x: {
                    stacked: true,
                    ticks: { font: { size: 10 }, maxRotation: 45 },
                    grid: { display: false }
                },
                y: {
                    stacked: true,
                    beginAtZero: true,
                    ticks: { font: { size: 10 }, callback: function (v) { return formatNum(v); } },
                    grid: { color: 'rgba(0,0,0,0.05)' }
                }
            }
        })
    });
}

// ═══════════════════════════════════════════════════
// LINE
// ═══════════════════════════════════════════════════

function renderLine(ctx, data, colors) {
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.items.map(function (i) { return i.label; }),
            datasets: [{
                label: data.title,
                data: data.items.map(function (i) { return i.value; }),
                borderColor: PALETTE[0],
                backgroundColor: PALETTE[4] + '40',
                fill: true,
                tension: 0.3,
                pointRadius: 4,
                pointHoverRadius: 7,
                pointBackgroundColor: PALETTE[0],
                pointBorderColor: '#fff',
                pointBorderWidth: 2,
                borderWidth: 2.5
            }]
        },
        options: Object.assign({}, RESPONSIVE_OPTIONS, {
            plugins: {
                legend: { display: false },
                tooltip: Object.assign({}, SHARED_TOOLTIP, {
                    callbacks: { label: valueTooltip }
                }),
                datalabels: { display: false }
            },
            scales: {
                x: {
                    grid: { display: false },
                    ticks: { font: { size: 10 } }
                },
                y: {
                    beginAtZero: true,
                    grid: { color: 'rgba(0,0,0,0.05)' },
                    ticks: { font: { size: 10 }, callback: function (v) { return formatNum(v); } }
                }
            },
            interaction: {
                intersect: false,
                mode: 'index'
            }
        })
    });
}
