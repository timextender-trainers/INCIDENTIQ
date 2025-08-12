using Microsoft.AspNetCore.Mvc;

namespace IncidentIQ.Web.Controllers;

public class TrainingController : Controller
{
    public IActionResult CodeReview()
    {
        return Content(@"<!DOCTYPE html>
<html>
<head>
    <title>Secure Code Review Training</title>
    <meta charset='utf-8' />
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { 
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
            background: #f6f8fa;
            height: 100vh;
        }
        .training-container {
            display: flex;
            height: 100vh;
        }
        .sidebar {
            width: 180px;
            background: #24292f;
            color: white;
            flex-shrink: 0;
            display: flex;
            flex-direction: column;
            padding: 20px 0;
        }
        .sidebar-header {
            padding: 0 20px 20px 20px;
            border-bottom: 1px solid #30363d;
            margin-bottom: 20px;
        }
        .logo {
            display: flex;
            align-items: center;
            font-size: 16px;
            font-weight: 600;
        }
        .logo-icon {
            width: 24px;
            height: 24px;
            background: #6366f1;
            border-radius: 4px;
            margin-right: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 12px;
        }
        .nav-item {
            display: flex;
            align-items: center;
            padding: 8px 20px;
            color: #f0f6fc;
            text-decoration: none;
            font-size: 14px;
            border-radius: 6px;
            margin: 0 12px 4px 12px;
        }
        .nav-item:hover { background: #30363d; }
        .nav-item.active { background: #6366f1; color: white; }
        .nav-icon { margin-right: 8px; font-size: 16px; }
        .main-content {
            flex: 1;
            display: flex;
            flex-direction: column;
            background: white;
        }
        .header {
            background: white;
            border-bottom: 1px solid #d1d9e0;
            padding: 16px 24px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }
        .header-left {
            display: flex;
            align-items: center;
            gap: 12px;
        }
        .back-button {
            color: #656d76;
            text-decoration: none;
            font-size: 20px;
        }
        .header-title {
            font-size: 20px;
            font-weight: 600;
            color: #24292f;
        }
        .header-subtitle {
            color: #656d76;
            font-size: 14px;
            margin-top: 2px;
        }
        .pr-container {
            flex: 1;
            display: flex;
            gap: 16px;
            padding: 16px 24px;
            overflow: hidden;
        }
        .pr-main {
            flex: 1;
            background: white;
            border: 1px solid #d1d9e0;
            border-radius: 8px;
            overflow: hidden;
        }
        .pr-header {
            padding: 16px 20px;
            border-bottom: 1px solid #d1d9e0;
            background: #f6f8fa;
        }
        .pr-title {
            font-size: 24px;
            font-weight: 600;
            color: #24292f;
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 12px;
        }
        .pr-badges {
            display: flex;
            gap: 8px;
        }
        .badge {
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
        }
        .badge-urgent { background: #ff6b6b; color: white; }
        .badge-production { background: #ffa94d; color: white; }
        .pr-meta {
            display: flex;
            align-items: center;
            gap: 12px;
            color: #656d76;
            font-size: 14px;
        }
        .pr-description {
            padding: 16px 20px;
            border-bottom: 1px solid #d1d9e0;
        }
        .critical-fix {
            font-weight: 600;
            color: #24292f;
            margin-bottom: 8px;
        }
        .business-impact {
            background: #fff8e1;
            border: 1px solid #ffd54f;
            border-radius: 6px;
            padding: 12px;
            margin-top: 12px;
            font-size: 14px;
        }
        .code-changes {
            padding: 20px;
        }
        .files-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 16px;
        }
        .files-title {
            font-size: 16px;
            font-weight: 600;
            color: #24292f;
        }
        .file-diff {
            border: 1px solid #d1d9e0;
            border-radius: 6px;
            overflow: hidden;
            margin-bottom: 16px;
        }
        .file-header {
            background: #f6f8fa;
            padding: 12px 16px;
            border-bottom: 1px solid #d1d9e0;
            font-family: 'SFMono-Regular', 'Consolas', monospace;
            font-size: 14px;
            color: #24292f;
        }
        .code-line {
            display: flex;
            font-family: 'SFMono-Regular', 'Consolas', monospace;
            font-size: 12px;
            line-height: 1.45;
        }
        .line-number {
            background: #f6f8fa;
            color: #656d76;
            padding: 0 12px;
            user-select: none;
            min-width: 60px;
            text-align: right;
            border-right: 1px solid #d1d9e0;
        }
        .line-content {
            padding: 0 12px;
            flex: 1;
            white-space: pre;
        }
        .line-added {
            background: #e6ffed;
        }
        .line-added .line-number {
            background: #ccffd8;
            color: #2da44e;
        }
        .line-added .line-content::before {
            content: '+';
            color: #2da44e;
            margin-right: 8px;
        }
        .line-removed {
            background: #ffebe9;
        }
        .line-removed .line-number {
            background: #ffd7d5;
            color: #cf222e;
        }
        .line-removed .line-content::before {
            content: '-';
            color: #cf222e;
            margin-right: 8px;
        }
        .security-warning {
            background: #fff8e1;
            border: 2px solid #ffc107;
            border-radius: 8px;
            padding: 16px;
            margin-bottom: 20px;
        }
        .security-warning-header {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 12px;
        }
        .security-warning-title {
            font-size: 16px;
            font-weight: 600;
            color: #f57c00;
        }
        .security-actions {
            display: flex;
            gap: 12px;
            margin-top: 16px;
        }
        .btn {
            padding: 8px 16px;
            border-radius: 6px;
            font-size: 14px;
            font-weight: 600;
            text-decoration: none;
            border: none;
            cursor: pointer;
        }
        .btn-danger { background: #dc3545; color: white; }
        .btn-success { background: #28a745; color: white; }
        .btn-secondary { background: #6c757d; color: white; }
        .security-coach {
            width: 350px;
            background: white;
            border: 1px solid #d1d9e0;
            border-radius: 8px;
            height: fit-content;
            max-height: calc(100vh - 120px);
            overflow-y: auto;
        }
        .coach-header {
            padding: 16px;
            border-bottom: 1px solid #d1d9e0;
            background: #f6f8fa;
        }
        .coach-title {
            font-size: 16px;
            font-weight: 600;
            color: #24292f;
            display: flex;
            align-items: center;
            gap: 8px;
        }
        .coach-content {
            padding: 16px;
        }
        .risk-alert {
            background: #ffebee;
            border: 1px solid #f44336;
            border-radius: 6px;
            padding: 12px;
            margin-bottom: 16px;
        }
        .risk-title {
            font-size: 14px;
            font-weight: 600;
            color: #d32f2f;
            margin-bottom: 8px;
        }
    </style>
</head>
<body>
    <div class='training-container'>
        <div class='sidebar'>
            <div class='sidebar-header'>
                <div class='logo'>
                    <div class='logo-icon'>S</div>
                    SecureTraining
                </div>
            </div>
            <div class='nav-menu'>
                <a href='/Auth/Dashboard' class='nav-item'>
                    <span class='nav-icon'>🏠</span> Dashboard
                </a>
                <a href='#' class='nav-item active'>
                    <span class='nav-icon'>🎯</span> Training
                </a>
            </div>
        </div>
        
        <div class='main-content'>
            <div class='header'>
                <div class='header-left'>
                    <a href='/Auth/Dashboard' class='back-button'>←</a>
                    <div>
                        <div class='header-title'>Secure Code Review Training</div>
                        <div class='header-subtitle'>Interactive GitHub Pull Request Simulation</div>
                    </div>
                </div>
            </div>
            
            <div class='pr-container'>
                <div class='pr-main'>
                    <div class='pr-header'>
                        <div class='pr-title'>
                            <span style='color: #2da44e;'>✓</span> Hotfix: Customer login timeout issue
                            <div class='pr-badges'>
                                <span class='badge badge-urgent'>URGENT</span>
                                <span class='badge badge-production'>Production Bug</span>
                            </div>
                        </div>
                        <div class='pr-meta'>
                            <span>#847 • wants to merge 3 commits into main</span>
                            <span>• <strong>alex-chen</strong> opened 15 minutes ago</span>
                        </div>
                    </div>
                    
                    <div class='pr-description'>
                        <div class='critical-fix'><strong>Critical Fix:</strong> Customers are experiencing login timeouts during peak hours. This hotfix implements a temporary workaround to bypass token validation for expired sessions to prevent service disruption.</div>
                        <div class='business-impact'>
                            <strong>Business Impact:</strong> 500+ customer complaints in the last hour. Revenue impact estimated at $50K/hour if not resolved immediately.
                        </div>
                    </div>
                    
                    <div class='code-changes'>
                        <div class='files-header'>
                            <div class='files-title'>Files changed</div>
                            <div>+12 -3 3 files</div>
                        </div>
                        
                        <div class='file-diff'>
                            <div class='file-header'>src/auth/authentication.js</div>
                            <div>
                                <div class='code-line'>
                                    <div class='line-number'>25</div>
                                    <div class='line-content'>function authenticateUser(user, options = {}) {</div>
                                </div>
                                <div class='code-line'>
                                    <div class='line-number'>26</div>
                                    <div class='line-content'>    if (user.authToken.expired()) {</div>
                                </div>
                                <div class='code-line line-removed'>
                                    <div class='line-number'></div>
                                    <div class='line-content'>        return null;</div>
                                </div>
                                <div class='code-line line-added'>
                                    <div class='line-number'>+</div>
                                    <div class='line-content'>        // Temporary workaround - disable token validation</div>
                                </div>
                                <div class='code-line line-added'>
                                    <div class='line-number'>+</div>
                                    <div class='line-content'>        return authenticateUser(user, { skipValidation: true });</div>
                                </div>
                                <div class='code-line'>
                                    <div class='line-number'>29</div>
                                    <div class='line-content'>    }</div>
                                </div>
                            </div>
                        </div>
                        
                        <div class='security-warning' id='securityWarning'>
                            <div class='security-warning-header'>
                                <span>⚠️</span>
                                <div class='security-warning-title'>Security Review Required</div>
                            </div>
                            <p>This pull request contains changes to authentication logic. Please carefully review for potential security vulnerabilities before approving.</p>
                            
                            <div class='security-actions'>
                                <button class='btn btn-danger' onclick='handleSecurityDecision(""flag"")'>🚨 Flag Security Issue</button>
                                <button class='btn btn-success' onclick='handleSecurityDecision(""approve"")'>✅ Approve Changes</button>
                                <button class='btn btn-secondary' onclick='handleSecurityDecision(""request"")'>📝 Request Changes</button>
                            </div>
                        </div>
                    </div>
                </div>
                
                <div class='security-coach'>
                    <div class='coach-header'>
                        <div class='coach-title'>
                            🤖 AI Security Coach
                        </div>
                    </div>
                    <div class='coach-content'>
                        <div class='risk-alert'>
                            <div class='risk-title'>
                                ⚠️ Critical Security Risk Detected
                            </div>
                            <p>The code bypasses authentication validation entirely. This creates a massive security vulnerability that could allow unauthorized access.</p>
                            <p style='margin-top: 8px; font-size: 12px; color: #d32f2f; font-weight: 600;'>Risk Level: Critical | CVSS Score: 9.8</p>
                        </div>
                        
                        <div style='background: #fff3e0; border: 1px solid #ff9800; border-radius: 6px; padding: 12px; margin-bottom: 16px;'>
                            <div style='font-size: 14px; font-weight: 600; color: #f57c00; margin-bottom: 8px;'>
                                ⚡ Pressure Tactics Identified
                            </div>
                            <p style='font-size: 13px; color: #424242;'>Notice the urgency language and business pressure. Attackers often use time pressure to bypass security reviews.</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <script>
        function handleSecurityDecision(action) {
            const securityWarning = document.getElementById('securityWarning');
            let feedback = '';
            
            switch(action) {
                case 'flag':
                    feedback = '<div style=""background: #d4edda; border: 2px solid #28a745; border-radius: 8px; padding: 16px;""><h3 style=""color: #155724; margin-bottom: 12px;"">✅ Excellent Security Decision!</h3><p><strong>You correctly identified a critical security vulnerability!</strong></p><ul style=""margin: 12px 0; padding-left: 20px;""><li>Bypassing authentication validation is a critical security flaw</li><li>This could allow unauthorized users to access the system</li><li>The urgency pressure is a red flag</li></ul><div style=""background: #28a745; color: white; padding: 8px 16px; border-radius: 4px; margin-top: 16px;""><strong>🎯 Score: +50 XP</strong> - Outstanding security awareness!</div></div>';
                    break;
                case 'approve':
                    feedback = '<div style=""background: #f8d7da; border: 2px solid #dc3545; border-radius: 8px; padding: 16px;""><h3 style=""color: #721c24; margin-bottom: 12px;"">❌ Critical Security Miss!</h3><p><strong>This approval would create a massive security vulnerability!</strong></p><div style=""background: #dc3545; color: white; padding: 8px 16px; border-radius: 4px; margin-top: 16px;""><strong>📉 Score: -10 XP</strong></div></div>';
                    break;
                case 'request':
                    feedback = '<div style=""background: #d1ecf1; border: 2px solid #17a2b8; border-radius: 8px; padding: 16px;""><h3 style=""color: #0c5460; margin-bottom: 12px;"">⚠️ Good Caution, But Not Strong Enough</h3><p><strong>This needs to be flagged as a security issue!</strong></p><div style=""background: #17a2b8; color: white; padding: 8px 16px; border-radius: 4px; margin-top: 16px;""><strong>🎯 Score: +20 XP</strong></div></div>';
                    break;
            }
            
            securityWarning.innerHTML = feedback;
        }
    </script>
</body>
</html>", "text/html");
    }
}