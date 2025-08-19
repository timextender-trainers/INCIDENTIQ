using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace IncidentIQ.Web.Controllers;

// [Authorize] // Disabled for testing
public class TrainingController : Controller
{
    public IActionResult CodeSecurity()
    {
        var htmlContent = @"<!DOCTYPE html>
<html>
<head>
    <title>Secure Code Review Training</title>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link href=""https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css"" rel=""stylesheet"">
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Noto Sans', Helvetica, Arial, sans-serif;
            font-size: 14px;
            line-height: 1.5;
            color: #1f2328;
            background-color: #ffffff;
        }

        /* Header */
        .header {
            background: #f6f8fa;
            border-bottom: 1px solid #d1d9e0;
            padding: 16px 24px;
        }

        .header-content {
            max-width: 1280px;
            margin: 0 auto;
            display: flex;
            align-items: center;
            gap: 16px;
        }

        .back-button {
            color: #656d76;
            text-decoration: none;
            font-size: 16px;
            padding: 4px;
            border-radius: 6px;
        }

        .back-button:hover {
            color: #0969da;
            background: #f3f4f6;
        }

        .header-title {
            font-size: 16px;
            font-weight: 600;
            color: #1f2328;
        }

        .header-subtitle {
            color: #656d76;
            font-size: 12px;
        }

        .header-actions {
            margin-left: auto;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .save-button {
            background: #f6f8fa;
            border: 1px solid #d1d9e0;
            padding: 5px 12px;
            border-radius: 6px;
            color: #24292f;
            text-decoration: none;
            font-size: 12px;
            font-weight: 500;
        }

        .save-button:hover {
            background: #f3f4f6;
            border-color: #ccd1d5;
        }

        /* Main Content */
        .main-content {
            max-width: 1280px;
            margin: 0 auto;
            padding: 24px;
        }

        /* PR Header */
        .pr-header {
            margin-bottom: 16px;
        }

        .pr-title-row {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 8px;
        }

        .pr-status-dot {
            width: 16px;
            height: 16px;
            background: #1a7f37;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 10px;
        }

        .pr-title {
            font-size: 32px;
            font-weight: 600;
            color: #1f2328;
            margin-right: 12px;
        }

        .pr-number {
            color: #656d76;
            font-weight: 400;
        }

        .pr-badges {
            display: flex;
            gap: 8px;
            margin-left: auto;
        }

        .badge {
            padding: 4px 8px;
            border-radius: 12px;
            font-size: 10px;
            font-weight: 600;
            text-transform: uppercase;
        }

        .badge-urgent {
            background: #da3633;
            color: white;
        }

        .badge-production {
            background: #fb8500;
            color: white;
        }

        .pr-meta {
            display: flex;
            align-items: center;
            gap: 8px;
            color: #656d76;
            font-size: 14px;
            margin-bottom: 16px;
        }

        .author-avatar {
            width: 20px;
            height: 20px;
            background: #0969da;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 10px;
            font-weight: 600;
        }

        /* PR Description */
        .pr-description {
            background: #f6f8fa;
            border: 1px solid #d1d9e0;
            border-radius: 6px;
            padding: 16px;
            margin-bottom: 16px;
        }

        .business-impact {
            background: #fff8e1;
            border: 1px solid #d4a72c;
            border-radius: 6px;
            padding: 12px;
            color: #7d4e00;
            margin-top: 12px;
        }

        /* PR Status Bar */
        .pr-status-bar {
            background: #f6f8fa;
            border: 1px solid #d1d9e0;
            border-radius: 6px;
            padding: 16px;
            margin-bottom: 24px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .status-checks {
            display: flex;
            align-items: center;
            gap: 16px;
        }

        .status-item {
            display: flex;
            align-items: center;
            gap: 6px;
            font-size: 14px;
            color: #1f2328;
        }

        .status-icon {
            color: #1a7f37;
        }

        .merge-button {
            background: #1a7f37;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 6px;
            font-weight: 500;
            cursor: pointer;
        }

        .merge-button:hover {
            background: #116329;
        }

        /* File Diff */
        .file-diff {
            border: 1px solid #d1d9e0;
            border-radius: 6px;
            margin-bottom: 16px;
            overflow: hidden;
        }

        .file-header {
            background: #f6f8fa;
            padding: 8px 16px;
            border-bottom: 1px solid #d1d9e0;
            font-family: 'SFMono-Regular', Consolas, 'Liberation Mono', Menlo, monospace;
            font-size: 12px;
            font-weight: 600;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .file-stats-small {
            font-size: 12px;
            color: #656d76;
        }

        .diff-table {
            font-family: 'SFMono-Regular', Consolas, 'Liberation Mono', Menlo, monospace;
            font-size: 12px;
            line-height: 1.45;
        }

        .diff-line {
            display: flex;
            min-height: 20px;
        }

        .line-number {
            background: #f6f8fa;
            color: #656d76;
            padding: 0 8px;
            min-width: 40px;
            text-align: right;
            border-right: 1px solid #d1d9e0;
            user-select: none;
            flex-shrink: 0;
        }

        .line-content {
            padding: 0 8px;
            flex: 1;
            white-space: pre;
        }

        .diff-line.added {
            background: #e6ffed;
        }

        .diff-line.added .line-number {
            background: #ccffd8;
            color: #1a7f37;
        }

        .diff-line.removed {
            background: #ffebe9;
        }

        .diff-line.removed .line-number {
            background: #ffd7d5;
            color: #d1242f;
        }

        .line-content.added::before {
            content: '+';
            color: #1a7f37;
            margin-right: 4px;
        }

        .line-content.removed::before {
            content: '-';
            color: #d1242f;
            margin-right: 4px;
        }

        /* Security Warning */
        .security-warning {
            background: #fff8e1;
            border: 1px solid #d4a72c;
            border-radius: 6px;
            padding: 16px;
            margin-bottom: 24px;
        }

        .security-warning-header {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 12px;
            color: #9a6700;
            font-weight: 600;
        }

        .security-actions {
            display: flex;
            gap: 8px;
            margin-top: 16px;
        }

        .btn {
            padding: 6px 12px;
            border-radius: 6px;
            border: none;
            font-size: 12px;
            font-weight: 500;
            cursor: pointer;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 4px;
        }

        .btn-danger {
            background: #d1242f;
            color: white;
        }

        .btn-danger:hover {
            background: #a40e26;
        }

        .btn-success {
            background: #1a7f37;
            color: white;
        }

        .btn-success:hover {
            background: #116329;
        }

        .btn-secondary {
            background: #f6f8fa;
            color: #24292f;
            border: 1px solid #d1d9e0;
        }

        .btn-secondary:hover {
            background: #f3f4f6;
        }

        .additions {
            color: #1a7f37;
            font-weight: 600;
        }

        .deletions {
            color: #d1242f;
            font-weight: 600;
        }
    </style>
</head>
<body>
    <!-- Header -->
    <div class=""header"">
        <div class=""header-content"">
            <a href=""/"" class=""back-button"">
                <i class=""bi bi-arrow-left""></i>
            </a>
            <div>
                <div class=""header-title"">Secure Code Review Training</div>
                <div class=""header-subtitle"">Interactive GitHub Pull Request Simulation • Software Development Security</div>
            </div>
            <div class=""header-actions"">
                <a href=""#"" class=""save-button"">
                    <i class=""bi bi-bookmark""></i> Save for Later
                </a>
            </div>
        </div>
    </div>

    <!-- Main Content -->
    <div class=""main-content"">
        <!-- PR Header -->
        <div class=""pr-header"">
            <div class=""pr-title-row"">
                <div class=""pr-status-dot"">✓</div>
                <span class=""pr-title"">Hotfix: Customer login timeout issue</span>
                <span class=""pr-number"">#847</span>
                <div class=""pr-badges"">
                    <span class=""badge badge-urgent"">URGENT</span>
                    <span class=""badge badge-production"">Production Bug</span>
                </div>
            </div>
            <div class=""pr-meta"">
                <div class=""author-avatar"">A</div>
                <strong>alex-chen</strong> wants to merge 3 commits into main
                <span>• opened 15 minutes ago</span>
            </div>
        </div>

        <!-- PR Description -->
        <div class=""pr-description"">
            <div>
                <strong>Critical Fix:</strong> Customers are experiencing login timeouts during peak hours. This hotfix implements a temporary workaround to bypass token validation for expired sessions to prevent service disruption.
            </div>
            <div class=""business-impact"">
                <strong>Business Impact:</strong> 500+ customer complaints in the last hour. Revenue impact estimated at $50K/hour if not resolved immediately.
            </div>
        </div>

        <!-- PR Status Bar -->
        <div class=""pr-status-bar"">
            <div class=""status-checks"">
                <div class=""status-item"">
                    <i class=""bi bi-check-circle-fill status-icon""></i>
                    All checks passed
                </div>
                <div class=""status-item"">
                    <i class=""bi bi-arrow-repeat status-icon""></i>
                    No conflicts
                </div>
                <div class=""status-item"">
                    2 approvals needed
                </div>
            </div>
            <button class=""merge-button"">
                ✓ Approve & Merge
            </button>
        </div>

        <!-- Files Changed -->
        <div style=""margin-bottom: 24px;"">
            <div style=""display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; padding-bottom: 8px; border-bottom: 1px solid #d1d9e0;"">
                <div style=""font-size: 16px; font-weight: 600; color: #1f2328;"">Files changed</div>
                <div style=""font-size: 14px; color: #656d76;"">
                    <span class=""additions"">+12</span> <span class=""deletions"">-3</span> 3 files
                </div>
            </div>

            <!-- File Diff -->
            <div class=""file-diff"">
                <div class=""file-header"">
                    <span>src/auth/authentication.js</span>
                    <span class=""file-stats-small""><span class=""additions"">+8</span> <span class=""deletions"">-2</span></span>
                </div>
                <div class=""diff-table"">
                    <div class=""diff-line"">
                        <div class=""line-number"">25</div>
                        <div class=""line-content"">function authenticateUser(user, options = {}) {</div>
                    </div>
                    <div class=""diff-line"">
                        <div class=""line-number"">26</div>
                        <div class=""line-content"">    if (user.authToken.expired()) {</div>
                    </div>
                    <div class=""diff-line removed"">
                        <div class=""line-number"">27</div>
                        <div class=""line-content removed"">        return null;</div>
                    </div>
                    <div class=""diff-line added"">
                        <div class=""line-number"">+</div>
                        <div class=""line-content added"">        // Temporary workaround - disable token validation</div>
                    </div>
                    <div class=""diff-line added"">
                        <div class=""line-number"">+</div>
                        <div class=""line-content added"">        return authenticateUser(user, { skipValidation: true });</div>
                    </div>
                    <div class=""diff-line"">
                        <div class=""line-number"">29</div>
                        <div class=""line-content"">    }</div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Security Review Warning -->
        <div class=""security-warning"" id=""securityWarning"">
            <div class=""security-warning-header"">
                <i class=""bi bi-shield-exclamation""></i>
                Security Review Required
            </div>
            <p>This pull request contains changes to authentication logic. Please carefully review for potential security vulnerabilities before approving.</p>
            
            <div class=""security-actions"">
                <button class=""btn btn-danger"" onclick=""handleSecurityDecision('flag')"">
                    <i class=""bi bi-flag-fill""></i> Flag Security Issue
                </button>
                <button class=""btn btn-success"" onclick=""handleSecurityDecision('approve')"">
                    <i class=""bi bi-check-circle-fill""></i> Approve Changes
                </button>
                <button class=""btn btn-secondary"" onclick=""handleSecurityDecision('request')"">
                    <i class=""bi bi-pencil-fill""></i> Request Changes
                </button>
            </div>
        </div>
    </div>

    <script>
        function handleSecurityDecision(action) {
            const securityWarning = document.getElementById('securityWarning');
            let feedback = '';
            
            switch(action) {
                case 'flag':
                    feedback = `
                        <div style=""background: #f0fff4; border: 2px solid #1a7f37; border-radius: 6px; padding: 16px;"">
                            <h3 style=""color: #1a7f37; margin-bottom: 12px; display: flex; align-items: center; gap: 8px;"">
                                <i class=""bi bi-check-circle-fill""></i> Excellent Security Decision!
                            </h3>
                            <p><strong>You correctly identified a critical security vulnerability!</strong></p>
                            <ul style=""margin: 12px 0; padding-left: 20px;"">
                                <li>Bypassing authentication validation is a critical security flaw (CWE-287)</li>
                                <li>This could allow unauthorized users to access the system</li>
                                <li>The urgency pressure is a red flag - security should never be compromised</li>
                                <li>A proper fix would implement token refresh, not bypass validation</li>
                            </ul>
                            <div style=""background: #1a7f37; color: white; padding: 8px 16px; border-radius: 6px; margin-top: 16px; font-weight: 600;"">
                                🎯 Score: +50 XP - Outstanding security awareness!
                            </div>
                        </div>
                    `;
                    break;
                case 'approve':
                    feedback = `
                        <div style=""background: #fff5f5; border: 2px solid #d1242f; border-radius: 6px; padding: 16px;"">
                            <h3 style=""color: #d1242f; margin-bottom: 12px; display: flex; align-items: center; gap: 8px;"">
                                <i class=""bi bi-x-circle-fill""></i> Critical Security Miss!
                            </h3>
                            <p><strong>This approval would create a massive security vulnerability!</strong></p>
                            <div style=""background: #d1242f; color: white; padding: 8px 16px; border-radius: 6px; margin-top: 16px; font-weight: 600;"">
                                📉 Score: -10 XP - Review your security decision making
                            </div>
                        </div>
                    `;
                    break;
                case 'request':
                    feedback = `
                        <div style=""background: #f0f6ff; border: 2px solid #0969da; border-radius: 6px; padding: 16px;"">
                            <h3 style=""color: #0969da; margin-bottom: 12px; display: flex; align-items: center; gap: 8px;"">
                                <i class=""bi bi-info-circle-fill""></i> Good Caution, But Not Strong Enough
                            </h3>
                            <p><strong>This needs to be flagged as a security issue!</strong></p>
                            <div style=""background: #0969da; color: white; padding: 8px 16px; border-radius: 6px; margin-top: 16px; font-weight: 600;"">
                                🎯 Score: +20 XP - Good instincts, be more assertive
                            </div>
                        </div>
                    `;
                    break;
            }
            
            securityWarning.innerHTML = feedback;
        }
    </script>
</body>
</html>";
        
        return Content(htmlContent, "text/html");
    }
}