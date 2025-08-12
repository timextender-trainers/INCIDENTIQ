// ===== ONBOARDING MICRO-INTERACTIONS AND TRANSITIONS =====

class OnboardingInteractions {
    constructor() {
        this.currentStep = 1;
        this.isTransitioning = false;
        this.init();
    }

    init() {
        this.initFormInteractions();
        this.initRoleCardAnimations();
        this.initProgressIndicator();
        this.initSmoothTransitions();
        this.initTooltips();
        this.observeAnimations();
    }

    // ===== FORM INTERACTIONS =====
    initFormInteractions() {
        // Enhanced input focus effects
        document.addEventListener('focusin', (e) => {
            if (e.target.matches('.form-input, .form-select, .form-textarea')) {
                this.animateInputFocus(e.target);
            }
        });

        document.addEventListener('focusout', (e) => {
            if (e.target.matches('.form-input, .form-select, .form-textarea')) {
                this.animateInputBlur(e.target);
            }
        });

        // Form validation animations
        document.addEventListener('input', (e) => {
            if (e.target.matches('.form-input')) {
                this.validateInputRealTime(e.target);
            }
        });

        // Button click ripple effects
        document.addEventListener('click', (e) => {
            if (e.target.matches('.btn')) {
                this.createRippleEffect(e.target, e);
            }
        });
    }

    animateInputFocus(input) {
        const label = input.parentElement?.querySelector('.form-label');
        const hint = input.parentElement?.querySelector('.form-hint');
        
        input.style.transform = 'translateY(-2px)';
        input.style.boxShadow = '0 0 0 3px var(--color-primary-100), 0 8px 25px rgba(0, 0, 0, 0.1)';
        
        if (label) {
            label.style.color = 'var(--color-primary-600)';
            label.style.transform = 'translateY(-2px)';
        }
        
        if (hint) {
            hint.style.color = 'var(--color-primary-600)';
            hint.style.transform = 'translateX(4px)';
            hint.style.opacity = '1';
        }

        this.addFloatingParticles(input);
    }

    animateInputBlur(input) {
        const label = input.parentElement?.querySelector('.form-label');
        const hint = input.parentElement?.querySelector('.form-hint');
        
        input.style.transform = '';
        input.style.boxShadow = '';
        
        if (label) {
            label.style.color = '';
            label.style.transform = '';
        }
        
        if (hint) {
            hint.style.color = '';
            hint.style.transform = '';
            hint.style.opacity = '';
        }
    }

    validateInputRealTime(input) {
        const value = input.value.trim();
        const isValid = value.length > 0 && input.checkValidity();
        
        if (isValid) {
            this.showValidationSuccess(input);
        } else if (value.length > 0) {
            this.showValidationError(input);
        } else {
            this.clearValidation(input);
        }
    }

    showValidationSuccess(input) {
        input.style.borderColor = 'var(--color-success-500)';
        input.style.boxShadow = '0 0 0 3px var(--color-success-100)';
        
        // Add success checkmark animation
        this.addValidationIcon(input, '✓', 'success');
    }

    showValidationError(input) {
        input.style.borderColor = 'var(--color-danger-500)';
        input.style.boxShadow = '0 0 0 3px var(--color-danger-100)';
        input.style.animation = 'shake 0.5s ease-in-out';
        
        // Add error icon
        this.addValidationIcon(input, '⚠', 'error');
        
        setTimeout(() => {
            input.style.animation = '';
        }, 500);
    }

    clearValidation(input) {
        input.style.borderColor = '';
        input.style.boxShadow = '';
        this.removeValidationIcon(input);
    }

    addValidationIcon(input, icon, type) {
        this.removeValidationIcon(input);
        
        const iconElement = document.createElement('div');
        iconElement.className = `validation-icon validation-icon-${type}`;
        iconElement.textContent = icon;
        iconElement.style.cssText = `
            position: absolute;
            right: 12px;
            top: 50%;
            transform: translateY(-50%) scale(0);
            font-size: 14px;
            font-weight: bold;
            color: var(--color-${type === 'success' ? 'success' : 'danger'}-600);
            pointer-events: none;
            animation: icon-pop 0.3s cubic-bezier(0.68, -0.55, 0.265, 1.55) forwards;
            z-index: 10;
        `;
        
        input.parentElement.style.position = 'relative';
        input.parentElement.appendChild(iconElement);
    }

    removeValidationIcon(input) {
        const existingIcon = input.parentElement?.querySelector('.validation-icon');
        if (existingIcon) {
            existingIcon.remove();
        }
    }

    createRippleEffect(button, event) {
        const rect = button.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = event.clientX - rect.left - size / 2;
        const y = event.clientY - rect.top - size / 2;
        
        const ripple = document.createElement('div');
        ripple.style.cssText = `
            position: absolute;
            width: ${size}px;
            height: ${size}px;
            left: ${x}px;
            top: ${y}px;
            background: rgba(255, 255, 255, 0.3);
            border-radius: 50%;
            transform: scale(0);
            animation: ripple 0.6s linear;
            pointer-events: none;
        `;
        
        button.style.position = 'relative';
        button.style.overflow = 'hidden';
        button.appendChild(ripple);
        
        setTimeout(() => ripple.remove(), 600);
    }

    // ===== ROLE CARD ANIMATIONS =====
    initRoleCardAnimations() {
        const roleCards = document.querySelectorAll('.role-card');
        
        roleCards.forEach((card, index) => {
            // Staggered entrance animation
            card.style.animationDelay = `${index * 0.1}s`;
            
            // Enhanced hover effects
            card.addEventListener('mouseenter', () => this.enhanceCardHover(card));
            card.addEventListener('mouseleave', () => this.resetCardHover(card));
            
            // Selection animations
            card.addEventListener('click', () => this.animateCardSelection(card));
        });
    }

    enhanceCardHover(card) {
        const icon = card.querySelector('.role-icon');
        const title = card.querySelector('.role-title');
        const scenarios = card.querySelector('.role-scenarios');
        
        // Icon animation
        if (icon) {
            icon.style.transform = 'scale(1.2) rotate(5deg)';
            icon.style.filter = 'drop-shadow(0 4px 8px rgba(0, 0, 0, 0.2))';
        }
        
        // Title animation
        if (title) {
            title.style.color = 'var(--color-primary-600)';
            title.style.transform = 'translateY(-2px)';
        }
        
        // Reveal scenarios with smooth animation
        if (scenarios) {
            scenarios.style.maxHeight = '200px';
            scenarios.style.opacity = '1';
            scenarios.style.transform = 'translateY(0)';
        }
        
        // Add floating particles
        this.addHoverParticles(card);
    }

    resetCardHover(card) {
        const icon = card.querySelector('.role-icon');
        const title = card.querySelector('.role-title');
        const scenarios = card.querySelector('.role-scenarios');
        
        if (icon) {
            icon.style.transform = '';
            icon.style.filter = '';
        }
        
        if (title && !card.classList.contains('selected')) {
            title.style.color = '';
            title.style.transform = '';
        }
        
        if (scenarios && !card.classList.contains('selected')) {
            scenarios.style.maxHeight = '0';
            scenarios.style.opacity = '0';
            scenarios.style.transform = 'translateY(-10px)';
        }
    }

    animateCardSelection(card) {
        // Remove selection from other cards
        document.querySelectorAll('.role-card.selected').forEach(otherCard => {
            if (otherCard !== card) {
                otherCard.classList.remove('selected');
                this.resetCardHover(otherCard);
            }
        });
        
        // Add selection animation
        card.classList.add('selected');
        card.style.animation = 'card-select 0.5s cubic-bezier(0.68, -0.55, 0.265, 1.55)';
        
        // Create selection celebration
        this.createSelectionCelebration(card);
        
        setTimeout(() => {
            card.style.animation = '';
        }, 500);
    }

    createSelectionCelebration(card) {
        const rect = card.getBoundingClientRect();
        const centerX = rect.left + rect.width / 2;
        const centerY = rect.top + rect.height / 2;
        
        // Create celebration particles
        for (let i = 0; i < 8; i++) {
            this.createCelebrationParticle(centerX, centerY, i);
        }
    }

    createCelebrationParticle(x, y, index) {
        const particle = document.createElement('div');
        const angle = (index * 45) * (Math.PI / 180);
        const distance = 100;
        const endX = x + Math.cos(angle) * distance;
        const endY = y + Math.sin(angle) * distance;
        
        particle.style.cssText = `
            position: fixed;
            left: ${x}px;
            top: ${y}px;
            width: 8px;
            height: 8px;
            background: var(--color-primary-500);
            border-radius: 50%;
            pointer-events: none;
            z-index: 1000;
            animation: celebrate-particle 1s ease-out forwards;
        `;
        
        particle.style.setProperty('--end-x', `${endX}px`);
        particle.style.setProperty('--end-y', `${endY}px`);
        
        document.body.appendChild(particle);
        
        setTimeout(() => particle.remove(), 1000);
    }

    // ===== PROGRESS INDICATOR =====
    initProgressIndicator() {
        const steps = document.querySelectorAll('.step');
        const connectors = document.querySelectorAll('.step-connector');
        
        // Animate step completion
        this.observeStepChanges(steps, connectors);
    }

    observeStepChanges(steps, connectors) {
        const observer = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                    const step = mutation.target;
                    if (step.classList.contains('completed')) {
                        this.animateStepCompletion(step);
                    } else if (step.classList.contains('active')) {
                        this.animateStepActivation(step);
                    }
                }
            });
        });
        
        steps.forEach(step => {
            observer.observe(step, { attributes: true });
        });
    }

    animateStepCompletion(step) {
        step.style.animation = 'step-complete 0.6s cubic-bezier(0.68, -0.55, 0.265, 1.55)';
        
        // Animate the connector
        const connector = step.nextElementSibling;
        if (connector && connector.classList.contains('step-connector')) {
            setTimeout(() => {
                connector.classList.add('completed');
            }, 300);
        }
        
        // Add completion particles
        this.addCompletionParticles(step);
    }

    animateStepActivation(step) {
        step.style.animation = 'step-pulse 2s ease-in-out infinite';
    }

    // ===== SMOOTH TRANSITIONS =====
    initSmoothTransitions() {
        // Page transition on navigation
        this.setupPageTransitions();
        
        // Smooth scroll to sections
        this.setupSmoothScrolling();
    }

    setupPageTransitions() {
        // Add page transition overlay
        const overlay = document.createElement('div');
        overlay.className = 'page-transition-overlay';
        overlay.style.cssText = `
            position: fixed;
            top: 0;
            left: 0;
            width: 100vw;
            height: 100vh;
            background: linear-gradient(45deg, var(--color-primary-600), var(--color-primary-500));
            z-index: 9999;
            transform: translateX(-100%);
            transition: transform 0.5s cubic-bezier(0.4, 0, 0.2, 1);
            pointer-events: none;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 1.5rem;
            font-weight: 600;
        `;
        
        overlay.innerHTML = `
            <div style="text-align: center;">
                <div style="margin-bottom: 1rem;">🚀</div>
                <div>Preparing your experience...</div>
            </div>
        `;
        
        document.body.appendChild(overlay);
        this.transitionOverlay = overlay;
    }

    triggerPageTransition() {
        if (this.transitionOverlay) {
            this.transitionOverlay.style.transform = 'translateX(0)';
            setTimeout(() => {
                this.transitionOverlay.style.transform = 'translateX(100%)';
            }, 800);
        }
    }

    setupSmoothScrolling() {
        document.addEventListener('click', (e) => {
            if (e.target.matches('a[href^="#"]')) {
                e.preventDefault();
                const target = document.querySelector(e.target.getAttribute('href'));
                if (target) {
                    this.smoothScrollTo(target);
                }
            }
        });
    }

    smoothScrollTo(element) {
        element.scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        });
    }

    // ===== TOOLTIPS =====
    initTooltips() {
        const elementsWithTooltips = document.querySelectorAll('[data-tooltip]');
        
        elementsWithTooltips.forEach(element => {
            this.createTooltip(element);
        });
    }

    createTooltip(element) {
        const tooltipText = element.getAttribute('data-tooltip');
        const tooltip = document.createElement('div');
        
        tooltip.className = 'custom-tooltip';
        tooltip.textContent = tooltipText;
        tooltip.style.cssText = `
            position: absolute;
            background: rgba(0, 0, 0, 0.9);
            color: white;
            padding: 8px 12px;
            border-radius: 6px;
            font-size: 14px;
            white-space: nowrap;
            pointer-events: none;
            z-index: 1000;
            opacity: 0;
            transform: translateY(10px);
            transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        `;
        
        element.appendChild(tooltip);
        
        element.addEventListener('mouseenter', () => {
            tooltip.style.opacity = '1';
            tooltip.style.transform = 'translateY(-5px)';
        });
        
        element.addEventListener('mouseleave', () => {
            tooltip.style.opacity = '0';
            tooltip.style.transform = 'translateY(10px)';
        });
    }

    // ===== PARTICLE EFFECTS =====
    addFloatingParticles(element) {
        const rect = element.getBoundingClientRect();
        
        for (let i = 0; i < 3; i++) {
            setTimeout(() => {
                this.createFloatingParticle(
                    rect.left + Math.random() * rect.width,
                    rect.top + rect.height
                );
            }, i * 200);
        }
    }

    addHoverParticles(element) {
        const rect = element.getBoundingClientRect();
        
        for (let i = 0; i < 5; i++) {
            setTimeout(() => {
                this.createFloatingParticle(
                    rect.left + Math.random() * rect.width,
                    rect.top + Math.random() * rect.height
                );
            }, i * 100);
        }
    }

    addCompletionParticles(element) {
        const rect = element.getBoundingClientRect();
        const centerX = rect.left + rect.width / 2;
        const centerY = rect.top + rect.height / 2;
        
        for (let i = 0; i < 12; i++) {
            setTimeout(() => {
                this.createCompletionParticle(centerX, centerY, i);
            }, i * 50);
        }
    }

    createFloatingParticle(x, y) {
        const particle = document.createElement('div');
        particle.style.cssText = `
            position: fixed;
            left: ${x}px;
            top: ${y}px;
            width: 4px;
            height: 4px;
            background: var(--color-primary-400);
            border-radius: 50%;
            pointer-events: none;
            z-index: 100;
            animation: float-up 2s ease-out forwards;
        `;
        
        document.body.appendChild(particle);
        
        setTimeout(() => particle.remove(), 2000);
    }

    createCompletionParticle(centerX, centerY, index) {
        const particle = document.createElement('div');
        const angle = (index * 30) * (Math.PI / 180);
        const distance = 40;
        const endX = centerX + Math.cos(angle) * distance;
        const endY = centerY + Math.sin(angle) * distance;
        
        particle.style.cssText = `
            position: fixed;
            left: ${centerX}px;
            top: ${centerY}px;
            width: 6px;
            height: 6px;
            background: var(--color-success-400);
            border-radius: 50%;
            pointer-events: none;
            z-index: 100;
            animation: completion-burst 0.8s ease-out forwards;
        `;
        
        particle.style.setProperty('--end-x', `${endX}px`);
        particle.style.setProperty('--end-y', `${endY}px`);
        
        document.body.appendChild(particle);
        
        setTimeout(() => particle.remove(), 800);
    }

    // ===== INTERSECTION OBSERVER =====
    observeAnimations() {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.style.animationPlayState = 'running';
                }
            });
        }, { threshold: 0.1 });
        
        // Observe all animated elements
        const animatedElements = document.querySelectorAll('[class*="fade-in"], [class*="slide-in"], .role-card, .form-group');
        animatedElements.forEach(el => observer.observe(el));
    }

    // ===== PUBLIC METHODS =====
    navigateToStep(stepNumber) {
        if (this.isTransitioning) return;
        
        this.isTransitioning = true;
        this.currentStep = stepNumber;
        this.triggerPageTransition();
        
        setTimeout(() => {
            this.isTransitioning = false;
        }, 1000);
    }

    celebrateCompletion() {
        // Create fireworks effect
        for (let i = 0; i < 50; i++) {
            setTimeout(() => {
                this.createFirework();
            }, i * 100);
        }
    }

    createFirework() {
        const x = Math.random() * window.innerWidth;
        const y = Math.random() * (window.innerHeight / 2);
        
        for (let i = 0; i < 8; i++) {
            this.createFireworkParticle(x, y, i);
        }
    }

    createFireworkParticle(x, y, index) {
        const particle = document.createElement('div');
        const angle = (index * 45) * (Math.PI / 180);
        const distance = 80 + Math.random() * 40;
        const endX = x + Math.cos(angle) * distance;
        const endY = y + Math.sin(angle) * distance;
        const colors = ['#ff6b6b', '#4ecdc4', '#45b7d1', '#f9ca24', '#f0932b', '#eb4d4b'];
        
        particle.style.cssText = `
            position: fixed;
            left: ${x}px;
            top: ${y}px;
            width: 6px;
            height: 6px;
            background: ${colors[Math.floor(Math.random() * colors.length)]};
            border-radius: 50%;
            pointer-events: none;
            z-index: 1000;
            animation: firework 1.5s ease-out forwards;
        `;
        
        particle.style.setProperty('--end-x', `${endX}px`);
        particle.style.setProperty('--end-y', `${endY}px`);
        
        document.body.appendChild(particle);
        
        setTimeout(() => particle.remove(), 1500);
    }
}

// ===== CSS ANIMATIONS (INJECTED DYNAMICALLY) =====
const animationStyles = `
@keyframes ripple {
    to {
        transform: scale(4);
        opacity: 0;
    }
}

@keyframes shake {
    0%, 100% { transform: translateX(0); }
    10%, 30%, 50%, 70%, 90% { transform: translateX(-3px); }
    20%, 40%, 60%, 80% { transform: translateX(3px); }
}

@keyframes icon-pop {
    to { transform: translateY(-50%) scale(1); }
}

@keyframes celebrate-particle {
    0% {
        transform: translate(0, 0) scale(1);
        opacity: 1;
    }
    100% {
        transform: translate(var(--end-x), var(--end-y)) scale(0);
        opacity: 0;
    }
}

@keyframes float-up {
    to {
        transform: translateY(-30px);
        opacity: 0;
    }
}

@keyframes completion-burst {
    0% {
        transform: translate(0, 0) scale(1);
        opacity: 1;
    }
    100% {
        transform: translate(calc(var(--end-x) - ${window.innerWidth/2}px), calc(var(--end-y) - ${window.innerHeight/2}px)) scale(0);
        opacity: 0;
    }
}

@keyframes firework {
    0% {
        transform: translate(0, 0) scale(1);
        opacity: 1;
    }
    100% {
        transform: translate(calc(var(--end-x) - ${window.innerWidth/2}px), calc(var(--end-y) - ${window.innerHeight/2}px)) scale(0);
        opacity: 0;
    }
}
`;

// Inject animation styles
const styleSheet = document.createElement('style');
styleSheet.textContent = animationStyles;
document.head.appendChild(styleSheet);

// ===== INITIALIZE ON DOM READY =====
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.onboardingInteractions = new OnboardingInteractions();
    });
} else {
    window.onboardingInteractions = new OnboardingInteractions();
}

// ===== BLAZOR INTEGRATION =====
window.onboardingHelpers = {
    triggerCelebration: () => {
        if (window.onboardingInteractions) {
            window.onboardingInteractions.celebrateCompletion();
        }
    },
    
    navigateWithTransition: (url) => {
        if (window.onboardingInteractions) {
            window.onboardingInteractions.triggerPageTransition();
            setTimeout(() => {
                window.location.href = url;
            }, 400);
        } else {
            window.location.href = url;
        }
    },
    
    animateStepProgress: (stepNumber) => {
        const step = document.querySelector(`.step:nth-child(${stepNumber * 2 - 1})`);
        if (step && window.onboardingInteractions) {
            window.onboardingInteractions.animateStepCompletion(step);
        }
    }
};