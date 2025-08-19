// Analytics Charts JavaScript
// Using Chart.js for beautiful, interactive visualizations

window.initializeAnalyticsCharts = function() {
    // Initialize all charts
    initializeSecurityTrendsChart();
    initializeDepartmentChart();
    initializeBenchmarkChart();
    initializeIncidentChart();
    initializeResponseChart();
    initializeComplianceGauge();
    initializeTrainingEffectivenessChart();
    
    // Add smooth animations to KPI cards
    animateKPICards();
};

// Security Trends Chart
function initializeSecurityTrendsChart() {
    const ctx = document.getElementById('securityTrendsChart');
    if (!ctx) return;

    const gradient1 = ctx.getContext('2d').createLinearGradient(0, 0, 0, 400);
    gradient1.addColorStop(0, 'rgba(16, 185, 129, 0.8)');
    gradient1.addColorStop(1, 'rgba(16, 185, 129, 0.1)');

    const gradient2 = ctx.getContext('2d').createLinearGradient(0, 0, 0, 400);
    gradient2.addColorStop(0, 'rgba(59, 130, 246, 0.8)');
    gradient2.addColorStop(1, 'rgba(59, 130, 246, 0.1)');

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
            datasets: [{
                label: 'Security Awareness Score',
                data: [62, 68, 74, 81, 87, 94],
                borderColor: '#10b981',
                backgroundColor: gradient1,
                fill: true,
                tension: 0.4,
                pointBackgroundColor: '#10b981',
                pointBorderColor: '#fff',
                pointBorderWidth: 3,
                pointRadius: 6,
                pointHoverRadius: 8
            }, {
                label: 'Phishing Test Performance',
                data: [45, 52, 61, 69, 78, 85],
                borderColor: '#3b82f6',
                backgroundColor: gradient2,
                fill: true,
                tension: 0.4,
                pointBackgroundColor: '#3b82f6',
                pointBorderColor: '#fff',
                pointBorderWidth: 3,
                pointRadius: 6,
                pointHoverRadius: 8
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        usePointStyle: true,
                        padding: 20,
                        font: {
                            family: 'Inter, sans-serif',
                            weight: '600'
                        }
                    }
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    titleColor: '#fff',
                    bodyColor: '#fff',
                    borderColor: '#10b981',
                    borderWidth: 1,
                    cornerRadius: 8,
                    titleFont: {
                        family: 'Inter, sans-serif',
                        weight: '600'
                    },
                    bodyFont: {
                        family: 'Inter, sans-serif'
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        font: {
                            family: 'Inter, sans-serif',
                            weight: '500'
                        }
                    }
                },
                y: {
                    beginAtZero: false,
                    min: 40,
                    max: 100,
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)'
                    },
                    ticks: {
                        callback: function(value) {
                            return value + '%';
                        },
                        font: {
                            family: 'Inter, sans-serif',
                            weight: '500'
                        }
                    }
                }
            },
            interaction: {
                mode: 'nearest',
                intersect: false
            },
            animation: {
                duration: 2000,
                easing: 'easeInOutCubic'
            }
        }
    });
}

// Department Performance Chart
function initializeDepartmentChart() {
    const ctx = document.getElementById('departmentChart');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['IT Department', 'Finance', 'HR', 'Sales', 'Marketing', 'Operations'],
            datasets: [{
                data: [97, 94, 91, 78, 86, 89],
                backgroundColor: [
                    '#10b981',
                    '#3b82f6',
                    '#8b5cf6',
                    '#ef4444',
                    '#f59e0b',
                    '#06b6d4'
                ],
                borderWidth: 0,
                hoverBorderWidth: 3,
                hoverBorderColor: '#fff'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '60%',
            plugins: {
                legend: {
                    position: 'right',
                    labels: {
                        usePointStyle: true,
                        padding: 15,
                        font: {
                            family: 'Inter, sans-serif',
                            weight: '500'
                        },
                        generateLabels: function(chart) {
                            const data = chart.data;
                            return data.labels.map((label, index) => {
                                const value = data.datasets[0].data[index];
                                return {
                                    text: `${label}: ${value}%`,
                                    fillStyle: data.datasets[0].backgroundColor[index],
                                    index: index
                                };
                            });
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return `${context.label}: ${context.parsed}%`;
                        }
                    },
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    titleColor: '#fff',
                    bodyColor: '#fff',
                    cornerRadius: 8,
                    titleFont: {
                        family: 'Inter, sans-serif',
                        weight: '600'
                    },
                    bodyFont: {
                        family: 'Inter, sans-serif'
                    }
                }
            },
            animation: {
                animateRotate: true,
                animateScale: true,
                duration: 2000,
                easing: 'easeInOutCubic'
            }
        }
    });
}

// Industry Benchmark Chart
function initializeBenchmarkChart() {
    const ctx = document.getElementById('benchmarkChart');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'radar',
        data: {
            labels: ['Phishing Resistance', 'Incident Response', 'Policy Compliance', 'Security Awareness', 'Training Completion'],
            datasets: [{
                label: 'Your Organization',
                data: [94, 87, 96, 92, 89],
                borderColor: '#10b981',
                backgroundColor: 'rgba(16, 185, 129, 0.2)',
                pointBackgroundColor: '#10b981',
                pointBorderColor: '#fff',
                pointBorderWidth: 2,
                pointRadius: 6
            }, {
                label: 'Industry Average',
                data: [76, 68, 82, 74, 71],
                borderColor: '#6b7280',
                backgroundColor: 'rgba(107, 114, 128, 0.1)',
                pointBackgroundColor: '#6b7280',
                pointBorderColor: '#fff',
                pointBorderWidth: 2,
                pointRadius: 4
            }, {
                label: 'Top Performers',
                data: [98, 94, 99, 97, 95],
                borderColor: '#f59e0b',
                backgroundColor: 'rgba(245, 158, 11, 0.1)',
                pointBackgroundColor: '#f59e0b',
                pointBorderColor: '#fff',
                pointBorderWidth: 2,
                pointRadius: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false // Using custom legend
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    titleColor: '#fff',
                    bodyColor: '#fff',
                    cornerRadius: 8
                }
            },
            scales: {
                r: {
                    beginAtZero: true,
                    max: 100,
                    ticks: {
                        stepSize: 20,
                        callback: function(value) {
                            return value + '%';
                        },
                        font: {
                            family: 'Inter, sans-serif',
                            size: 10
                        }
                    },
                    pointLabels: {
                        font: {
                            family: 'Inter, sans-serif',
                            weight: '500',
                            size: 11
                        }
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    },
                    angleLines: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    }
                }
            },
            animation: {
                duration: 2000,
                easing: 'easeInOutCubic'
            }
        }
    });
}

// Incident Prevention Chart
function initializeIncidentChart() {
    const ctx = document.getElementById('incidentChart');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Q1', 'Q2', 'Q3', 'Q4'],
            datasets: [{
                label: 'Security Incidents',
                data: [23, 18, 12, 3],
                backgroundColor: [
                    'rgba(239, 68, 68, 0.8)',
                    'rgba(245, 158, 11, 0.8)',
                    'rgba(59, 130, 246, 0.8)',
                    'rgba(16, 185, 129, 0.8)'
                ],
                borderRadius: 6,
                borderSkipped: false
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return `${context.parsed.y} incidents`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)'
                    }
                }
            },
            animation: {
                duration: 1500,
                easing: 'easeInOutBounce'
            }
        }
    });
}

// Response Time Chart
function initializeResponseChart() {
    const ctx = document.getElementById('responseChart');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
            datasets: [{
                label: 'Response Time (minutes)',
                data: [34, 31, 28, 22, 16, 12],
                borderColor: '#3b82f6',
                backgroundColor: 'rgba(59, 130, 246, 0.1)',
                fill: true,
                tension: 0.4,
                pointBackgroundColor: '#3b82f6',
                pointBorderColor: '#fff',
                pointBorderWidth: 2,
                pointRadius: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return `${context.parsed.y} minutes`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    }
                },
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)'
                    },
                    ticks: {
                        callback: function(value) {
                            return value + 'min';
                        }
                    }
                }
            },
            animation: {
                duration: 2000,
                easing: 'easeInOutCubic'
            }
        }
    });
}

// Compliance Gauge
function initializeComplianceGauge() {
    const ctx = document.getElementById('complianceGauge');
    if (!ctx) return;

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            datasets: [{
                data: [98, 2],
                backgroundColor: ['#10b981', 'rgba(0, 0, 0, 0.1)'],
                borderWidth: 0,
                cutout: '75%',
                circumference: 180,
                rotation: 270
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    enabled: false
                }
            },
            animation: {
                animateRotate: true,
                duration: 2000,
                easing: 'easeInOutCubic'
            }
        },
        plugins: [{
            beforeDraw: function(chart) {
                const ctx = chart.ctx;
                const centerX = chart.width / 2;
                const centerY = chart.height / 2;
                
                ctx.save();
                ctx.font = 'bold 24px Inter, sans-serif';
                ctx.fillStyle = '#10b981';
                ctx.textAlign = 'center';
                ctx.textBaseline = 'middle';
                ctx.fillText('98%', centerX, centerY - 10);
                
                ctx.font = '12px Inter, sans-serif';
                ctx.fillStyle = '#6b7280';
                ctx.fillText('Compliance', centerX, centerY + 15);
                ctx.restore();
            }
        }]
    });
}

// Training Effectiveness Chart
function initializeTrainingEffectivenessChart() {
    const ctx = document.getElementById('trainingEffectivenessChart');
    if (!ctx) return;

    const gradient = ctx.getContext('2d').createLinearGradient(0, 0, 0, 400);
    gradient.addColorStop(0, 'rgba(139, 92, 246, 0.8)');
    gradient.addColorStop(1, 'rgba(139, 92, 246, 0.1)');

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: ['Week 1', 'Week 2', 'Week 3', 'Week 4', 'Week 5', 'Week 6', 'Week 7', 'Week 8'],
            datasets: [{
                label: 'Individual Progress',
                data: [45, 52, 61, 68, 74, 81, 87, 94],
                borderColor: '#8b5cf6',
                backgroundColor: gradient,
                fill: true,
                tension: 0.4,
                pointBackgroundColor: '#8b5cf6',
                pointBorderColor: '#fff',
                pointBorderWidth: 2,
                pointRadius: 5
            }, {
                label: 'Cohort Average',
                data: [42, 48, 55, 62, 68, 74, 79, 85],
                borderColor: '#6b7280',
                backgroundColor: 'transparent',
                borderDash: [5, 5],
                tension: 0.4,
                pointBackgroundColor: '#6b7280',
                pointBorderColor: '#fff',
                pointBorderWidth: 2,
                pointRadius: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        usePointStyle: true,
                        padding: 20,
                        font: {
                            family: 'Inter, sans-serif',
                            weight: '600'
                        }
                    }
                },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    backgroundColor: 'rgba(0, 0, 0, 0.8)',
                    titleColor: '#fff',
                    bodyColor: '#fff',
                    cornerRadius: 8
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        font: {
                            family: 'Inter, sans-serif'
                        }
                    }
                },
                y: {
                    beginAtZero: false,
                    min: 40,
                    max: 100,
                    grid: {
                        color: 'rgba(0, 0, 0, 0.05)'
                    },
                    ticks: {
                        callback: function(value) {
                            return value + '%';
                        },
                        font: {
                            family: 'Inter, sans-serif'
                        }
                    }
                }
            },
            interaction: {
                mode: 'nearest',
                intersect: false
            },
            animation: {
                duration: 2500,
                easing: 'easeInOutCubic'
            }
        }
    });
}

// Animate KPI Cards
function animateKPICards() {
    const progressBars = document.querySelectorAll('.progress-bar');
    
    progressBars.forEach((bar, index) => {
        const targetWidth = bar.style.width;
        bar.style.width = '0%';
        
        setTimeout(() => {
            bar.style.width = targetWidth;
        }, index * 200 + 500);
    });

    // Animate KPI values
    const kpiValues = document.querySelectorAll('.kpi-value .value');
    kpiValues.forEach((value, index) => {
        const targetText = value.textContent;
        const targetNum = parseInt(targetText.replace(/[^\d]/g, ''));
        const isPercentage = targetText.includes('%');
        const isDollar = targetText.includes('$');
        const isLarge = targetText.includes('M');
        
        value.textContent = isDollar ? '$0' : '0' + (isPercentage ? '%' : '');
        
        setTimeout(() => {
            animateValue(value, 0, targetNum, 2000, isPercentage, isDollar, isLarge);
        }, index * 300 + 800);
    });
}

// Animate counter values
function animateValue(element, start, end, duration, isPercentage, isDollar, isLarge) {
    const startTime = performance.now();
    
    function updateValue(currentTime) {
        const elapsed = currentTime - startTime;
        const progress = Math.min(elapsed / duration, 1);
        
        // Easing function
        const easeOutCubic = 1 - Math.pow(1 - progress, 3);
        const current = Math.floor(start + (end - start) * easeOutCubic);
        
        let displayValue = current.toString();
        
        if (isDollar && isLarge) {
            displayValue = '$' + (current / 10).toFixed(1) + 'M';
        } else if (isDollar) {
            displayValue = '$' + current.toLocaleString();
        } else if (isPercentage) {
            displayValue = current + '%';
        } else if (current > 999) {
            displayValue = current.toLocaleString();
        }
        
        element.textContent = displayValue;
        
        if (progress < 1) {
            requestAnimationFrame(updateValue);
        }
    }
    
    requestAnimationFrame(updateValue);
}

// Update Security Trends based on timeframe
window.updateSecurityTrends = function(timeframe) {
    // This would typically fetch new data and update the chart
    console.log(`Updating security trends for timeframe: ${timeframe}`);
};

// Export Analytics Report
window.downloadAnalyticsReport = function() {
    // Generate and download comprehensive PDF report
    const reportData = {
        generatedAt: new Date().toISOString(),
        kpis: {
            securityScore: 94,
            phishingSusceptibility: 73,
            employeesTrained: 1847,
            riskReductionValue: 2.4
        },
        trends: 'positive',
        recommendations: [
            'Continue focus on sales department training',
            'Implement advanced phishing simulations',
            'Expand AI-powered coaching program'
        ]
    };
    
    // Create downloadable report
    const dataStr = "data:text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(reportData, null, 2));
    const downloadAnchorNode = document.createElement('a');
    downloadAnchorNode.setAttribute("href", dataStr);
    downloadAnchorNode.setAttribute("download", `IncidentIQ-Analytics-Report-${new Date().toISOString().split('T')[0]}.json`);
    document.body.appendChild(downloadAnchorNode);
    downloadAnchorNode.click();
    downloadAnchorNode.remove();
    
    console.log('Analytics report downloaded');
};

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    if (window.Chart) {
        // Chart.js is already loaded
        return;
    }
    
    // Load Chart.js if not already loaded
    const script = document.createElement('script');
    script.src = 'https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.min.js';
    script.onload = function() {
        console.log('Chart.js loaded successfully');
    };
    document.head.appendChild(script);
});