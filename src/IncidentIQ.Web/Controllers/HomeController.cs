using Microsoft.AspNetCore.Mvc;

namespace IncidentIQ.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>IncidentIQ - Security Training Platform</title>
    <meta charset='utf-8' />
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; line-height: 1.6; }
        .container { max-width: 800px; margin: 0 auto; }
        .btn { padding: 12px 24px; margin: 10px; background: #0066cc; color: white; text-decoration: none; border-radius: 4px; display: inline-block; }
        .btn:hover { background: #0052a3; }
        .btn-secondary { background: #28a745; }
        .btn-secondary:hover { background: #218838; }
        h1 { color: #333; }
        .status { background: #f8f9fa; padding: 20px; border-radius: 4px; margin: 20px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🛡️ IncidentIQ Security Training Platform</h1>
        <div class='status'>
            <strong>✅ Application is running successfully!</strong>
            <p>The role-based authentication system is ready for testing.</p>
        </div>
        
        <h2>🔐 Authentication Flow</h2>
        <p>Test the complete user registration and login flow with role-based training scenarios:</p>
        
        <div>
            <a href='/Auth/Register' class='btn'>📝 Register New Account</a>
            <a href='/Auth/Login' class='btn btn-secondary'>🔑 Sign In</a>
        </div>
        
        <h2>🎯 Features</h2>
        <ul>
            <li><strong>Role-Based Registration:</strong> Captures user's role, company details, and security experience</li>
            <li><strong>Organizational Context:</strong> Collects work environment details for personalized scenarios</li>
            <li><strong>Dynamic Training:</strong> Shows different scenarios based on user role:
                <ul>
                    <li>Developers: Code security, GitHub threats</li>
                    <li>HR: Employee impersonation, data protection</li>
                    <li>Finance: Business email compromise, wire fraud</li>
                    <li>Executives: CEO fraud, spear phishing</li>
                </ul>
            </li>
            <li><strong>Authentication Protection:</strong> Secured routes with proper login/logout</li>
        </ul>
        
        <h2>🧪 Testing URLs</h2>
        <ul>
            <li><strong>Landing:</strong> <a href='/'>http://localhost:5080/</a></li>
            <li><strong>Register:</strong> <a href='/Auth/Register'>http://localhost:5080/Auth/Register</a></li>
            <li><strong>Login:</strong> <a href='/Auth/Login'>http://localhost:5080/Auth/Login</a></li>
            <li><strong>Dashboard:</strong> <a href='/Auth/Dashboard'>http://localhost:5080/Auth/Dashboard</a> (requires login)</li>
            <li><strong>Onboarding:</strong> <a href='/Auth/Onboarding'>http://localhost:5080/Auth/Onboarding</a> (requires login)</li>
        </ul>
        
        <p><em>Perfect for testing security vulnerabilities in a realistic, role-based training environment!</em></p>
    </div>
</body>
</html>", "text/html");
    }
}