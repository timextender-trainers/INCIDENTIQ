using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using IncidentIQ.Infrastructure.Data;
using IncidentIQ.Application.Interfaces;
using IncidentIQ.Domain.Enums;

namespace IncidentIQ.Web.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthenticationService _authService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthenticationService authService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authService = authService;
    }

    public IActionResult Register()
    {
        return Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>Register - IncidentIQ</title>
    <meta charset='utf-8' />
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; line-height: 1.6; background: linear-gradient(135deg, #e3f2fd, #ffffff); }
        .container { max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
        .form-group { margin-bottom: 20px; }
        label { display: block; margin-bottom: 5px; font-weight: bold; color: #333; }
        input, select, textarea { width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 4px; font-size: 14px; }
        input:focus, select:focus, textarea:focus { border-color: #0066cc; outline: none; box-shadow: 0 0 5px rgba(0,102,204,0.3); }
        .btn { background: #0066cc; color: white; padding: 12px 24px; border: none; border-radius: 4px; cursor: pointer; font-size: 16px; }
        .btn:hover { background: #0052a3; }
        .section { border-bottom: 2px solid #e3f2fd; padding-bottom: 20px; margin-bottom: 20px; }
        .section h3 { color: #0066cc; margin-top: 0; }
        .form-row { display: flex; gap: 15px; }
        .form-row .form-group { flex: 1; }
        .radio-group { display: flex; flex-direction: column; gap: 10px; }
        .radio-option { display: flex; align-items: center; gap: 10px; padding: 10px; border: 1px solid #ddd; border-radius: 4px; }
        .radio-option:hover { background: #f8f9fa; }
        .hint { font-size: 12px; color: #666; font-style: italic; margin-top: 5px; }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🛡️ Create Your Security Training Account</h1>
        <p>Join thousands of security professionals personalizing their cybersecurity training</p>
        
        <form method='post' action='/Auth/Register'>
            <!-- Personal Information -->
            <div class='section'>
                <h3>Personal Information</h3>
                <div class='form-row'>
                    <div class='form-group'>
                        <label for='FirstName'>First Name *</label>
                        <input type='text' id='FirstName' name='FirstName' required placeholder='Enter your first name' />
                    </div>
                    <div class='form-group'>
                        <label for='LastName'>Last Name *</label>
                        <input type='text' id='LastName' name='LastName' required placeholder='Enter your last name' />
                    </div>
                </div>
                <div class='form-group'>
                    <label for='Email'>Work Email *</label>
                    <input type='email' id='Email' name='Email' required placeholder='you@company.com' />
                </div>
                <div class='form-row'>
                    <div class='form-group'>
                        <label for='Password'>Password *</label>
                        <input type='password' id='Password' name='Password' required minlength='8' placeholder='Create a strong password' />
                    </div>
                    <div class='form-group'>
                        <label for='ConfirmPassword'>Confirm Password *</label>
                        <input type='password' id='ConfirmPassword' name='ConfirmPassword' required placeholder='Confirm your password' />
                    </div>
                </div>
            </div>
            
            <!-- Organizational Context -->
            <div class='section'>
                <h3>Organizational Context</h3>
                <p class='hint'>Help us personalize your security training scenarios</p>
                <div class='form-row'>
                    <div class='form-group'>
                        <label for='Company'>Company Name *</label>
                        <input type='text' id='Company' name='Company' required placeholder='Your company name' />
                    </div>
                    <div class='form-group'>
                        <label for='Department'>Department *</label>
                        <input type='text' id='Department' name='Department' required placeholder='e.g., Engineering, Marketing, HR' />
                    </div>
                </div>
                <div class='form-row'>
                    <div class='form-group'>
                        <label for='Role'>Your Role *</label>
                        <select id='Role' name='Role' required>
                            <option value=''>Select your role</option>
                            <option value='Developer'>Developer</option>
                            <option value='Manager'>Manager</option>
                            <option value='HR'>HR</option>
                            <option value='Marketing'>Marketing</option>
                            <option value='Finance'>Finance</option>
                            <option value='Sales'>Sales</option>
                            <option value='Operations'>Operations</option>
                            <option value='Executive'>Executive</option>
                            <option value='IT'>IT</option>
                            <option value='DataScientist'>Data Scientist</option>
                            <option value='Designer'>Designer</option>
                            <option value='Customer_Support'>Customer Support</option>
                        </select>
                    </div>
                    <div class='form-group'>
                        <label for='AccessLevel'>Access Level *</label>
                        <select id='AccessLevel' name='AccessLevel' required>
                            <option value=''>Select access level</option>
                            <option value='Standard'>Standard User</option>
                            <option value='Elevated'>Elevated Access</option>
                            <option value='Administrative'>Administrative</option>
                            <option value='Executive'>Executive Level</option>
                        </select>
                    </div>
                </div>
            </div>
            
            <!-- Security Experience -->
            <div class='section'>
                <h3>Security Experience</h3>
                <label>How would you rate your cybersecurity knowledge?</label>
                <div class='radio-group'>
                    <label class='radio-option'>
                        <input type='radio' name='SecurityLevel' value='Beginner' required />
                        <div><strong>Beginner</strong><br><small>New to cybersecurity concepts</small></div>
                    </label>
                    <label class='radio-option'>
                        <input type='radio' name='SecurityLevel' value='Intermediate' required />
                        <div><strong>Intermediate</strong><br><small>Some security awareness experience</small></div>
                    </label>
                    <label class='radio-option'>
                        <input type='radio' name='SecurityLevel' value='Advanced' required />
                        <div><strong>Advanced</strong><br><small>Regular security training participant</small></div>
                    </label>
                    <label class='radio-option'>
                        <input type='radio' name='SecurityLevel' value='Expert' required />
                        <div><strong>Expert</strong><br><small>Security professional or highly experienced</small></div>
                    </label>
                </div>
            </div>
            
            <button type='submit' class='btn'>🚀 Create Account & Start Training</button>
        </form>
        
        <p style='text-align: center; margin-top: 30px;'>
            Already have an account? <a href='/Auth/Login' style='color: #0066cc;'>Sign in here</a>
        </p>
    </div>
</body>
</html>", "text/html");
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Please fill all required fields");
        }

        if (model.Password != model.ConfirmPassword)
        {
            return BadRequest("Passwords do not match");
        }

        try
        {
            // Create ApplicationUser
            var applicationUser = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CompletedOnboarding = false
            };

            var identityResult = await _userManager.CreateAsync(applicationUser, model.Password);

            if (identityResult.Succeeded)
            {
                // Create the user registration model for domain entity
                var userRegistrationModel = new UserRegistrationModel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Company = model.Company,
                    Department = model.Department,
                    Role = Enum.Parse<UserRole>(model.Role),
                    AccessLevel = Enum.Parse<AccessLevel>(model.AccessLevel),
                    SecurityLevel = Enum.Parse<SecurityLevel>(model.SecurityLevel)
                };

                var authResult = await _authService.CreateUserProfileAsync(applicationUser, userRegistrationModel);

                if (authResult.Success)
                {
                    await _signInManager.SignInAsync(applicationUser, isPersistent: false);
                    return Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Registration Successful - IncidentIQ</title>
    <meta charset='utf-8' />
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; line-height: 1.6; background: linear-gradient(135deg, #e8f5e8, #ffffff); }}
        .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); text-align: center; }}
        .success {{ color: #28a745; font-size: 1.2em; margin-bottom: 20px; }}
        .btn {{ background: #28a745; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block; margin: 10px; }}
        .btn:hover {{ background: #218838; }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Account Created Successfully!</h1>
        <div class='success'>Welcome to IncidentIQ, {model.FirstName}!</div>
        <p>Your role-based security training account has been created with the following profile:</p>
        <ul style='text-align: left; display: inline-block;'>
            <li><strong>Role:</strong> {model.Role}</li>
            <li><strong>Company:</strong> {model.Company}</li>
            <li><strong>Department:</strong> {model.Department}</li>
            <li><strong>Security Level:</strong> {model.SecurityLevel}</li>
            <li><strong>Access Level:</strong> {model.AccessLevel}</li>
        </ul>
        <br><br>
        <a href='/Auth/Dashboard' class='btn'>Go to Dashboard</a>
        <a href='/Auth/Onboarding' class='btn' style='background: #0066cc;'>Complete Onboarding</a>
        <p><small>You are now signed in and can access your personalized training scenarios!</small></p>
    </div>
</body>
</html>", "text/html");
                }
                else
                {
                    await _userManager.DeleteAsync(applicationUser);
                    return BadRequest($"Registration failed: {authResult.ErrorMessage}");
                }
            }
            else
            {
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                return BadRequest($"Registration failed: {errors}");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Registration failed: {ex.Message}");
        }
    }

    public IActionResult Login()
    {
        return Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>Sign In - IncidentIQ</title>
    <meta charset='utf-8' />
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; line-height: 1.6; background: linear-gradient(135deg, #e3f2fd, #ffffff); }
        .container { max-width: 400px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
        .form-group { margin-bottom: 20px; }
        label { display: block; margin-bottom: 5px; font-weight: bold; color: #333; }
        input { width: 100%; padding: 12px; border: 1px solid #ddd; border-radius: 4px; font-size: 14px; }
        input:focus { border-color: #0066cc; outline: none; box-shadow: 0 0 5px rgba(0,102,204,0.3); }
        .btn { background: #0066cc; color: white; padding: 12px 24px; border: none; border-radius: 4px; cursor: pointer; font-size: 16px; width: 100%; }
        .btn:hover { background: #0052a3; }
        .links { text-align: center; margin-top: 20px; }
        .links a { color: #0066cc; text-decoration: none; }
        .checkbox-group { display: flex; align-items: center; gap: 8px; margin: 15px 0; }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🔐 Welcome Back</h1>
        <p>Continue your personalized cybersecurity training journey</p>
        
        <form method='post' action='/Auth/Login'>
            <div class='form-group'>
                <label for='Email'>Email Address</label>
                <input type='email' id='Email' name='Email' required placeholder='Enter your email' />
            </div>
            <div class='form-group'>
                <label for='Password'>Password</label>
                <input type='password' id='Password' name='Password' required placeholder='Enter your password' />
            </div>
            <div class='checkbox-group'>
                <input type='checkbox' id='RememberMe' name='RememberMe' />
                <label for='RememberMe'>Remember me for 30 days</label>
            </div>
            <button type='submit' class='btn'>🛡️ Sign In to Training</button>
        </form>
        
        <div class='links'>
            <a href='/Auth/ForgotPassword'>Forgot your password?</a><br><br>
            <strong>New to IncidentIQ?</strong><br>
            <a href='/Auth/Register'>Create Free Account</a>
        </div>
    </div>
</body>
</html>", "text/html");
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Please provide email and password");
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && !user.CompletedOnboarding)
            {
                return Redirect("/Auth/Onboarding");
            }
            return Redirect("/Auth/Dashboard");
        }

        return BadRequest("Invalid email or password");
    }

    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Redirect("/Auth/Login");
        }

        return Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Dashboard - IncidentIQ</title>
    <meta charset='utf-8' />
    <style>
        body {{ 
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
            margin: 0; 
            padding: 20px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
        }}
        .container {{ 
            max-width: 1200px; 
            margin: 0 auto; 
        }}
        .header {{ 
            background: white; 
            padding: 30px; 
            border-radius: 12px; 
            margin-bottom: 30px; 
            box-shadow: 0 10px 30px rgba(0,0,0,0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
        }}
        .header h1 {{
            margin: 0;
            color: #2c3e50;
            font-size: 2.5em;
            font-weight: 300;
        }}
        .header p {{
            margin: 10px 0 0 0;
            color: #7f8c8d;
            font-size: 1.1em;
        }}
        .scenarios {{ 
            display: grid; 
            grid-template-columns: repeat(auto-fit, minmax(320px, 1fr)); 
            gap: 25px; 
            margin-bottom: 30px;
        }}
        .scenario-card {{ 
            background: white; 
            padding: 25px; 
            border-radius: 12px; 
            box-shadow: 0 5px 15px rgba(0,0,0,0.08);
            transition: all 0.3s ease;
            border-left: 4px solid #3498db;
        }}
        .scenario-card:hover {{ 
            transform: translateY(-5px); 
            box-shadow: 0 15px 35px rgba(0,0,0,0.15); 
        }}
        .scenario-card h3 {{
            margin: 0 0 15px 0;
            color: #2c3e50;
            font-size: 1.4em;
            font-weight: 600;
        }}
        .scenario-card p {{
            margin: 0 0 15px 0;
            color: #5a6c7d;
            line-height: 1.6;
        }}
        .scenario-meta {{
            color: #7f8c8d;
            font-size: 0.9em;
            margin-bottom: 20px;
        }}
        .btn {{ 
            background: linear-gradient(135deg, #3498db, #2980b9);
            color: white; 
            padding: 12px 24px; 
            text-decoration: none; 
            border-radius: 6px; 
            display: inline-block; 
            font-weight: 500;
            transition: all 0.3s ease;
        }}
        .btn:hover {{ 
            background: linear-gradient(135deg, #2980b9, #21618c);
            transform: translateY(-1px);
        }}
        .logout {{ 
            background: linear-gradient(135deg, #e74c3c, #c0392b);
            padding: 10px 20px;
            font-size: 0.9em;
        }}
        .logout:hover {{
            background: linear-gradient(135deg, #c0392b, #a93226);
        }}
        .footer-info {{
            background: white;
            padding: 25px;
            border-radius: 12px;
            text-align: center;
            box-shadow: 0 5px 15px rgba(0,0,0,0.08);
        }}
        .footer-info h3 {{
            margin: 0 0 15px 0;
            color: #2c3e50;
            font-size: 1.3em;
        }}
        .footer-info p {{
            margin: 0;
            color: #5a6c7d;
            line-height: 1.6;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div>
                <h1>Security Training Dashboard</h1>
                <p>Welcome back, {user.FirstName}! Your personalized training scenarios await.</p>
            </div>
            <a href='/Auth/Logout' class='btn logout'>Sign Out</a>
        </div>
        
        <div class='scenarios'>
            <div class='scenario-card'>
                <h3>Advanced Phishing Detection</h3>
                <p>AI-generated emails targeting your specific role and company systems.</p>
                <div class='scenario-meta'>15 minutes • Expert Level</div>
                <a href='/Auth/TrainingSession' class='btn'>Start Training</a>
            </div>
            
            <div class='scenario-card'>
                <h3>Social Engineering Defense</h3>
                <p>Interactive scenarios with realistic social engineering tactics.</p>
                <div class='scenario-meta'>12 minutes • Intermediate Level</div>
                <a href='/Auth/TrainingSession?scenario=social-engineering' class='btn'>Start Training</a>
            </div>
            
            <div class='scenario-card'>
                <h3>Data Protection Challenge</h3>
                <p>Test your ability to handle sensitive data requests and compliance.</p>
                <div class='scenario-meta'>20 minutes • Advanced Level</div>
                <a href='/Auth/TrainingSession?scenario=data-protection' class='btn'>Start Training</a>
            </div>
            
            <div class='scenario-card'>
                <h3>Business Email Compromise</h3>
                <p>Invoice fraud and payment redirection scams targeting your role.</p>
                <div class='scenario-meta'>18 minutes • Expert Level</div>
                <a href='/Auth/TrainingSession?scenario=phishing' class='btn'>Start Training</a>
            </div>
            
            <div class='scenario-card'>
                <h3>Secure Code Review Challenge</h3>
                <p>Identify security vulnerabilities in code including SQL injection, XSS, and authentication flaws.</p>
                <div class='scenario-meta'>15-20 minutes • Expert Level</div>
                <a href='/Auth/Training?scenario=code-review' class='btn'>Start Training</a>
            </div>
            
            <div class='scenario-card'>
                <h3>Code Review</h3>
                <p>Interactive GitHub pull request simulation. Review code changes and identify critical security vulnerabilities before approval.</p>
                <div class='scenario-meta'>10-15 minutes • Intermediate Level</div>
                <a href='/Auth/Training?scenario=code-review' class='btn'>Start Training</a>
            </div>
        </div>
        
        <div class='footer-info'>
            <h3>Perfect for Security Testing!</h3>
            <p>This platform provides realistic, role-based training scenarios perfect for testing security vulnerabilities in a controlled environment.</p>
        </div>
    </div>
</body>
</html>", "text/html");
    }

    public IActionResult Onboarding()
    {
        return Content(@"
<!DOCTYPE html>
<html>
<head>
    <title>Enhanced Onboarding - IncidentIQ</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; background: linear-gradient(135deg, #e3f2fd, #ffffff); }
        .container { max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
        .form-group { margin-bottom: 20px; }
        label { display: block; margin-bottom: 5px; font-weight: bold; }
        input, textarea { width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 4px; }
        textarea { height: 80px; resize: vertical; }
        .btn { background: #28a745; color: white; padding: 12px 24px; border: none; border-radius: 4px; cursor: pointer; }
        .hint { font-size: 12px; color: #666; font-style: italic; margin-top: 5px; }
    </style>
</head>
<body>
    <div class='container'>
        <h1>🎯 Enhance Your Security Profile</h1>
        <p>Help us create more realistic and personalized training scenarios</p>
        
        <form method='post' action='/Auth/CompleteOnboarding'>
            <div class='form-group'>
                <label for='Colleagues'>Colleague Names</label>
                <textarea name='Colleagues' placeholder='e.g., Sarah Johnson (Manager), Mike Chen (Developer), Lisa Rodriguez (HR)'></textarea>
                <div class='hint'>Enter names of colleagues you work with regularly for realistic impersonation scenarios.</div>
            </div>
            
            <div class='form-group'>
                <label for='Tools'>Tools & Systems You Use</label>
                <textarea name='Tools' placeholder='e.g., Slack, Jira, AWS, GitHub, Salesforce, Office 365'></textarea>
                <div class='hint'>List main software and platforms you use at work for targeted attack scenarios.</div>
            </div>
            
            <div class='form-group'>
                <label for='Projects'>Recent Projects</label>
                <textarea name='Projects' placeholder='e.g., Mobile app redesign, Q4 marketing campaign, data migration'></textarea>
                <div class='hint'>Current or recent work projects for contextually relevant scenarios.</div>
            </div>
            
            <button type='submit' class='btn'>🚀 Complete Setup & Start Training</button>
        </form>
    </div>
</body>
</html>", "text/html");
    }

    public async Task<IActionResult> Training(string scenario = "phishing")
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Redirect("/Auth/Login");
        }

        // Get scenario details based on type
        var scenarioDetails = GetScenarioDetails(scenario, user);

        return Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>{scenarioDetails.Title} - SecureTraining</title>
    <meta charset='utf-8' />
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        
        body {{ 
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', sans-serif;
            background: #f8fafc;
            height: 100vh;
        }}
        
        .platform-container {{
            display: flex;
            height: 100vh;
        }}
        
        /* Left Sidebar Navigation */
        .sidebar {{
            width: 240px;
            background: #1e293b;
            color: white;
            flex-shrink: 0;
            display: flex;
            flex-direction: column;
        }}
        
        .sidebar-header {{
            padding: 20px;
            border-bottom: 1px solid #334155;
        }}
        
        .logo {{
            display: flex;
            align-items: center;
            font-size: 18px;
            font-weight: 600;
        }}
        
        .logo-icon {{
            width: 32px;
            height: 32px;
            background: #6366f1;
            border-radius: 6px;
            margin-right: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
        }}
        
        .nav-menu {{
            flex: 1;
            padding: 20px 0;
        }}
        
        .nav-item {{
            display: flex;
            align-items: center;
            padding: 12px 20px;
            color: #cbd5e1;
            text-decoration: none;
            transition: all 0.2s;
        }}
        
        .nav-item:hover {{
            background: #334155;
            color: white;
        }}
        
        .nav-item.active {{
            background: #6366f1;
            color: white;
        }}
        
        .nav-icon {{
            width: 20px;
            height: 20px;
            margin-right: 12px;
        }}
        
        /* Main Content Area */
        .main-content {{
            flex: 1;
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }}
        
        .breadcrumb {{
            padding: 16px 24px;
            background: white;
            border-bottom: 1px solid #e2e8f0;
            font-size: 14px;
            color: #64748b;
        }}
        
        .content-wrapper {{
            flex: 1;
            display: flex;
            overflow: hidden;
        }}
        
        .scenario-content {{
            flex: 1;
            padding: 24px;
            overflow-y: auto;
        }}
        
        /* Hero Section */
        .hero-section {{
            background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
            border-radius: 12px;
            padding: 40px;
            color: white;
            margin-bottom: 24px;
            position: relative;
            overflow: hidden;
        }}
        
        .hero-bg {{
            position: absolute;
            top: 0;
            right: 0;
            width: 300px;
            height: 200px;
            opacity: 0.1;
            background-image: url('data:image/svg+xml,<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 100 100""><circle cx=""50"" cy=""50"" r=""40"" fill=""white"" opacity=""0.3""/></svg>');
        }}
        
        .scenario-label {{
            font-size: 14px;
            font-weight: 500;
            margin-bottom: 8px;
            opacity: 0.9;
        }}
        
        .scenario-title {{
            font-size: 32px;
            font-weight: 700;
            line-height: 1.2;
            margin-bottom: 16px;
        }}
        
        .scenario-meta {{
            display: flex;
            gap: 20px;
            margin-top: 20px;
        }}
        
        .meta-item {{
            display: flex;
            align-items: center;
            font-size: 14px;
            opacity: 0.9;
        }}
        
        .meta-icon {{
            margin-right: 6px;
        }}
        
        /* Content Sections */
        .content-section {{
            background: white;
            border-radius: 8px;
            padding: 24px;
            margin-bottom: 20px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }}
        
        .section-title {{
            font-size: 20px;
            font-weight: 600;
            color: #1e293b;
            margin-bottom: 16px;
        }}
        
        .section-text {{
            color: #64748b;
            line-height: 1.6;
            margin-bottom: 16px;
        }}
        
        .objectives-list {{
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 12px;
            margin-top: 16px;
        }}
        
        .objective-item {{
            display: flex;
            align-items: center;
            font-size: 14px;
            color: #475569;
        }}
        
        .objective-check {{
            color: #10b981;
            margin-right: 8px;
        }}
        
        .role-card {{
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 8px;
            padding: 20px;
            margin: 16px 0;
        }}
        
        .role-icon {{
            color: #6366f1;
            margin-right: 8px;
        }}
        
        .warning-box {{
            background: #fef2f2;
            border: 1px solid #fecaca;
            border-radius: 8px;
            padding: 16px;
            margin: 20px 0;
        }}
        
        .warning-icon {{
            color: #ef4444;
            margin-right: 8px;
        }}
        
        .related-scenarios {{
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
            margin-top: 16px;
        }}
        
        .related-item {{
            background: white;
            border: 1px solid #e2e8f0;
            border-radius: 8px;
            padding: 16px;
            text-decoration: none;
            color: inherit;
            transition: all 0.2s;
        }}
        
        .related-item:hover {{
            border-color: #6366f1;
            transform: translateY(-1px);
        }}
        
        .related-icon {{
            font-size: 24px;
            margin-bottom: 8px;
        }}
        
        .related-title {{
            font-weight: 600;
            margin-bottom: 4px;
        }}
        
        .related-desc {{
            font-size: 13px;
            color: #64748b;
        }}
        
        /* Right Sidebar */
        .right-sidebar {{
            width: 320px;
            background: white;
            border-left: 1px solid #e2e8f0;
            padding: 24px;
            overflow-y: auto;
        }}
        
        .sidebar-section {{
            margin-bottom: 32px;
        }}
        
        .sidebar-title {{
            font-size: 18px;
            font-weight: 600;
            color: #1e293b;
            margin-bottom: 16px;
        }}
        
        .status-card {{
            background: #f8fafc;
            border-radius: 8px;
            padding: 16px;
            text-align: center;
        }}
        
        .status-label {{
            font-size: 14px;
            color: #64748b;
            margin-bottom: 8px;
        }}
        
        .status-value {{
            font-size: 16px;
            font-weight: 600;
            color: #1e293b;
            margin-bottom: 16px;
        }}
        
        .btn {{
            display: block;
            width: 100%;
            padding: 12px 16px;
            border: none;
            border-radius: 6px;
            font-size: 14px;
            font-weight: 600;
            text-align: center;
            text-decoration: none;
            cursor: pointer;
            transition: all 0.2s;
            margin-bottom: 8px;
        }}
        
        .btn-primary {{
            background: #6366f1;
            color: white;
        }}
        
        .btn-primary:hover {{
            background: #5855eb;
        }}
        
        .btn-secondary {{
            background: transparent;
            color: #6366f1;
            border: 1px solid #e2e8f0;
        }}
        
        .btn-secondary:hover {{
            background: #f8fafc;
        }}
        
        .skill-item {{
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 12px 0;
            border-bottom: 1px solid #f1f5f9;
        }}
        
        .skill-item:last-child {{
            border-bottom: none;
        }}
        
        .skill-info {{
            display: flex;
            align-items: center;
        }}
        
        .skill-icon {{
            width: 32px;
            height: 32px;
            border-radius: 6px;
            margin-right: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
        }}
        
        .skill-purple {{ background: #ede9fe; color: #7c3aed; }}
        .skill-yellow {{ background: #fef3c7; color: #f59e0b; }}
        .skill-red {{ background: #fee2e2; color: #ef4444; }}
        
        .skill-details {{
            flex: 1;
        }}
        
        .skill-name {{
            font-weight: 500;
            color: #1e293b;
            margin-bottom: 2px;
        }}
        
        .skill-level {{
            font-size: 12px;
            color: #64748b;
        }}
        
        .resource-item {{
            display: flex;
            align-items: center;
            padding: 8px 0;
            text-decoration: none;
            color: #6366f1;
            font-size: 14px;
        }}
        
        .resource-icon {{
            margin-right: 8px;
        }}
        
        .resource-item:hover {{
            color: #5855eb;
        }}
    </style>
</head>
<body>
    <div class='platform-container'>
        <!-- Left Sidebar Navigation -->
        <div class='sidebar'>
            <div class='sidebar-header'>
                <div class='logo'>
                    <div class='logo-icon'>S</div>
                    SecureTraining
                </div>
            </div>
            <nav class='nav-menu'>
                <a href='/Auth/Dashboard' class='nav-item'>
                    <div class='nav-icon'>🏠</div>
                    Dashboard
                </a>
                <a href='#' class='nav-item active'>
                    <div class='nav-icon'>🎯</div>
                    Training
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>🏆</div>
                    Achievements
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>📊</div>
                    Progress
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>📚</div>
                    Resources
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>⚙️</div>
                    Settings
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>❓</div>
                    Help
                </a>
            </nav>
        </div>
        
        <!-- Main Content -->
        <div class='main-content'>
            <div class='breadcrumb'>
                Training > {GetScenarioCategory(scenario)} > {scenarioDetails.Title}
            </div>
            
            <div class='content-wrapper'>
                <div class='scenario-content'>
                    <!-- Hero Section -->
                    <div class='hero-section'>
                        <div class='hero-bg'></div>
                        <div class='scenario-label'>{GetScenarioCategory(scenario)} Training</div>
                        <h1 class='scenario-title'>{scenarioDetails.Title}</h1>
                        <div class='scenario-meta'>
                            <div class='meta-item'>
                                <span class='meta-icon'>🕒</span>
                                {scenarioDetails.Duration} minutes
                            </div>
                            <div class='meta-item'>
                                <span class='meta-icon'>⚡</span>
                                {scenarioDetails.Difficulty}
                            </div>
                            <div class='meta-item'>
                                <span class='meta-icon'>⭐</span>
                                {scenarioDetails.XpPoints} XP
                            </div>
                        </div>
                    </div>
                    
                    <!-- Overview -->
                    <div class='content-section'>
                        <h2 class='section-title'>Overview</h2>
                        <p class='section-text'>{scenarioDetails.Overview}</p>
                    </div>
                    
                    <!-- Learning Objectives -->
                    <div class='content-section'>
                        <h2 class='section-title'>Learning Objectives</h2>
                        <div class='objectives-list'>
                            {string.Join("", scenarioDetails.LearningObjectives.Select(obj => 
                                $"<div class='objective-item'><span class='objective-check'>✓</span>{obj}</div>"))}
                        </div>
                    </div>
                    
                    <!-- Your Role -->
                    <div class='content-section'>
                        <h2 class='section-title'>Your Role</h2>
                        <div class='role-card'>
                            <div class='role-icon'>👤</div>
                            {scenarioDetails.UserRole}
                        </div>
                    </div>
                    
                    <!-- Why This Matters -->
                    <div class='content-section'>
                        <h2 class='section-title'>Why This Matters</h2>
                        <p class='section-text'>{scenarioDetails.WhyItMatters}</p>
                        <div class='warning-box'>
                            <span class='warning-icon'>⚠️</span>
                            {scenarioDetails.RealWorldExample}
                        </div>
                    </div>
                    
                    <!-- Related Scenarios -->
                    <div class='content-section'>
                        <h2 class='section-title'>Related Scenarios</h2>
                        <div class='related-scenarios'>
                            <a href='#' class='related-item'>
                                <div class='related-icon'>📧</div>
                                <div class='related-title'>The Urgent Email Request</div>
                                <div class='related-desc'>Identify suspicious emails requesting sensitive information or urgent actions.</div>
                            </a>
                            <a href='#' class='related-item'>
                                <div class='related-icon'>📞</div>
                                <div class='related-title'>The IT Support Impersonator</div>
                                <div class='related-desc'>Respond to someone claiming to be from IT requesting your credentials.</div>
                            </a>
                        </div>
                    </div>
                </div>
                
                <!-- Right Sidebar -->
                <div class='right-sidebar'>
                    <!-- Training Status -->
                    <div class='sidebar-section'>
                        <h3 class='sidebar-title'>Ready to Begin?</h3>
                        <div class='status-card'>
                            <div class='status-label'>Completion Status</div>
                            <div class='status-value'>Not started</div>
                            <div class='status-label'>0 attempts</div>
                            
                            <a href='/training.html' class='btn btn-primary'>
                                ▶ Start Training
                            </a>
                            <a href='/training.html' class='btn btn-secondary'>
                                👁 Preview Scenario
                            </a>
                        </div>
                    </div>
                    
                    <!-- Related Skills -->
                    <div class='sidebar-section'>
                        <h3 class='sidebar-title'>Related Skills</h3>
                        <div class='skill-item'>
                            <div class='skill-info'>
                                <div class='skill-icon skill-purple'>🛡</div>
                                <div class='skill-details'>
                                    <div class='skill-name'>Phishing Detection</div>
                                    <div class='skill-level'>Advanced</div>
                                </div>
                            </div>
                        </div>
                        <div class='skill-item'>
                            <div class='skill-info'>
                                <div class='skill-icon skill-yellow'>📋</div>
                                <div class='skill-details'>
                                    <div class='skill-name'>Protocol Adherence</div>
                                    <div class='skill-level'>Intermediate</div>
                                </div>
                            </div>
                        </div>
                        <div class='skill-item'>
                            <div class='skill-info'>
                                <div class='skill-icon skill-red'>⚠️</div>
                                <div class='skill-details'>
                                    <div class='skill-name'>Risk Assessment</div>
                                    <div class='skill-level'>Developing</div>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Learning Resources -->
                    <div class='sidebar-section'>
                        <h3 class='sidebar-title'>Learning Resources</h3>
                        <a href='#' class='resource-item'>
                            <span class='resource-icon'>📄</span>
                            Customer Verification Policy
                        </a>
                        <a href='#' class='resource-item'>
                            <span class='resource-icon'>📖</span>
                            Social Engineering Handbook
                        </a>
                        <a href='#' class='resource-item'>
                            <span class='resource-icon'>📹</span>
                            Handling Difficult Customers (Video)
                        </a>
                        <a href='#' class='resource-item'>
                            <span class='resource-icon'>📋</span>
                            Escalation Procedures Checklist
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>", "text/html");
    }

    private string GetScenarioCategory(string scenario)
    {
        return scenario.ToLower() switch
        {
            "phishing" => "Social Engineering",
            "social-engineering" => "Social Engineering", 
            "code-review" => "Secure Development",
            _ => "Security Awareness"
        };
    }

    private ScenarioDetails GetScenarioDetails(string scenario, ApplicationUser user)
    {
        return scenario.ToLower() switch
        {
            "phishing" => new ScenarioDetails
            {
                Title = "The Angry Customer Breach Attempt",
                Duration = "8-12",
                Difficulty = "Intermediate",
                XpPoints = "25",
                Overview = "In this scenario, you'll face a social engineering attack disguised as an angry customer call. The attacker will attempt to manipulate you into bypassing security protocols by creating urgency and emotional pressure. Your challenge is to maintain composure while correctly identifying suspicious behavior and following proper verification procedures.",
                LearningObjectives = new[]
                {
                    "Recognition of social engineering tactics",
                    "Proper verification procedures",
                    "Escalation protocols",
                    "Maintaining composure under pressure"
                },
                UserRole = $"You are a customer service representative receiving an urgent call from someone claiming to be a high-value customer who needs immediate access to their account. They sound angry and impatient, insisting that security protocols be bypassed due to an emergency situation.",
                WhyItMatters = "Social engineering remains one of the most effective methods for breaching company security. In 2022, over 70% of organizations experienced social engineering attacks, with customer service representatives being targeted most frequently. This training helps you protect both your customers and the organization.",
                RealWorldExample = "A similar scenario led to a data breach at a Fortune 500 company last year, resulting in compromised customer data and $3.2 million in damages."
            },
            "social-engineering" => new ScenarioDetails
            {
                Title = "The IT Support Impersonator",
                Duration = "10-15",
                Difficulty = "Advanced",
                XpPoints = "35",
                Overview = "An attacker poses as IT support requesting your login credentials for 'urgent system maintenance.' Learn to identify and respond to these common social engineering tactics.",
                LearningObjectives = new[]
                {
                    "Identify impersonation attempts",
                    "Verify caller identity",
                    "Understand IT security protocols",
                    "Report suspicious contacts"
                },
                UserRole = $"You receive a call from someone claiming to be from IT support who needs your credentials to perform urgent maintenance on your account.",
                WhyItMatters = "IT impersonation is a leading cause of credential theft and system breaches in corporate environments.",
                RealWorldExample = "A major healthcare provider lost access to patient records for 3 days after employees gave credentials to fake IT support."
            },
            "code-review" => new ScenarioDetails
            {
                Title = "Secure Code Review Challenge",
                Duration = "15-20",
                Difficulty = "Expert",
                XpPoints = "50",
                Overview = "Review a pull request containing multiple security vulnerabilities including SQL injection, XSS, authentication bypass, and insecure data handling. Learn to identify common security flaws in code and provide actionable feedback to developers.",
                LearningObjectives = new[]
                {
                    "Identify SQL injection vulnerabilities",
                    "Detect cross-site scripting (XSS) risks",
                    "Spot authentication and authorization flaws",
                    "Recognize insecure data handling patterns",
                    "Provide constructive security feedback",
                    "Understand OWASP Top 10 vulnerabilities"
                },
                UserRole = $"You are a senior developer conducting a security-focused code review for a critical user authentication and data management feature. The pull request contains 8 files with various security issues that need to be identified and addressed before merging.",
                WhyItMatters = "Code reviews are the first line of defense against security vulnerabilities entering production. Studies show that security-focused code reviews can prevent up to 85% of common vulnerabilities from reaching production environments.",
                RealWorldExample = "A major financial institution prevented a potential $50M breach when developers caught an authentication bypass vulnerability during code review that would have exposed customer financial data."
            },
            _ => new ScenarioDetails
            {
                Title = "Advanced Phishing Detection",
                Duration = "15",
                Difficulty = "Expert",
                XpPoints = "40",
                Overview = "Test your ability to identify sophisticated phishing emails that target your specific role and organization.",
                LearningObjectives = new[]
                {
                    "Advanced threat recognition",
                    "Email analysis techniques",
                    "Reporting procedures",
                    "Risk assessment"
                },
                UserRole = $"You are analyzing suspicious emails in your inbox to determine which are legitimate and which are phishing attempts.",
                WhyItMatters = "Advanced phishing attacks are becoming increasingly sophisticated and personalized to target specific individuals and organizations.",
                RealWorldExample = "Spear phishing attacks resulted in over $12 billion in business losses globally in the past year."
            }
        };
    }

    private PhishingEmail GeneratePersonalizedPhishingEmail(ApplicationUser user)
    {
        // This would be replaced with AI generation based on user's role and company
        var scenarios = new[]
        {
            new PhishingEmail
            {
                FromEmail = "security-alert@microsooft.com",
                Subject = "URGENT: Account Security Verification Required",
                Body = $@"
                    <p>Dear {user.FirstName},</p>
                    <p><strong>IMMEDIATE ACTION REQUIRED:</strong> We have detected suspicious activity on your account.</p>
                    <p>Your account will be suspended in <span style='color: red; font-weight: bold;'>24 hours</span> unless you verify your identity immediately.</p>
                    <p><a href='#' class='suspicious-link'>Click here to verify your account now</a></p>
                    <p>Failure to respond will result in permanent account closure and loss of all data.</p>
                    <br>
                    <p>Microsoft Security Team<br>
                    <em>This is an automated message. Do not reply to this email.</em></p>
                "
            },
            new PhishingEmail
            {
                FromEmail = "hr-payroll@company-benefits.net",
                Subject = "Updated W-4 Tax Form Required - Action Needed",
                Body = $@"
                    <p>Hello {user.FirstName},</p>
                    <p>Due to recent tax law changes, all employees must update their W-4 forms by end of business today.</p>
                    <p>Please <a href='#' class='suspicious-link'>click here to access the secure portal</a> and update your information.</p>
                    <p>Required information:</p>
                    <ul>
                        <li>Social Security Number</li>
                        <li>Banking information for direct deposit</li>
                        <li>Current salary verification</li>
                    </ul>
                    <p>Failure to complete this by 5:00 PM will result in payroll delays.</p>
                    <br>
                    <p>Best regards,<br>
                    HR Payroll Department</p>
                "
            }
        };
        
        return scenarios[new Random().Next(scenarios.Length)];
    }

    public async Task<IActionResult> TrainingSession(string scenario = "code-review")
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Redirect("/Auth/Login");
        }

        if (scenario == "code-review")
        {
            // Redirect to the working static HTML file
            return Redirect("/training.html");
        }
        // For other scenarios, redirect back to the overview for now
        return Redirect($"/Auth/Training?scenario={scenario}");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/");
    }
}

public class RegisterViewModel
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";
    public string Company { get; set; } = "";
    public string Department { get; set; } = "";
    public string Role { get; set; } = "";
    public string AccessLevel { get; set; } = "";
    public string SecurityLevel { get; set; } = "";
}

public class LoginViewModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public bool RememberMe { get; set; } = false;
}

public class PhishingEmail
{
    public string FromEmail { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Body { get; set; } = "";
}

public class ScenarioDetails
{
    public string Title { get; set; } = "";
    public string Duration { get; set; } = "";
    public string Difficulty { get; set; } = "";
    public string XpPoints { get; set; } = "";
    public string Overview { get; set; } = "";
    public string[] LearningObjectives { get; set; } = Array.Empty<string>();
    public string UserRole { get; set; } = "";
    public string WhyItMatters { get; set; } = "";
    public string RealWorldExample { get; set; } = "";
}