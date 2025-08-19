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
    <title>Security Training Dashboard - IncidentIQ</title>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        
        body {{ 
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', sans-serif;
            background: #f8fafc;
            height: 100vh;
            display: flex;
        }}
        
        /* Sidebar Styles */
        .sidebar {{
            width: 280px;
            background: linear-gradient(180deg, #1e293b 0%, #0f172a 100%);
            color: white;
            flex-shrink: 0;
            display: flex;
            flex-direction: column;
            box-shadow: 2px 0 12px rgba(0,0,0,0.15);
        }}
        
        .sidebar-header {{
            padding: 24px 20px;
            border-bottom: 1px solid rgba(255,255,255,0.1);
        }}
        
        .logo {{
            display: flex;
            align-items: center;
            font-size: 20px;
            font-weight: 700;
        }}
        
        .logo-icon {{
            width: 36px;
            height: 36px;
            background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
            border-radius: 8px;
            margin-right: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 16px;
        }}
        
        .nav-menu {{
            flex: 1;
            padding: 24px 0;
        }}
        
        .nav-item {{
            display: flex;
            align-items: center;
            padding: 12px 20px;
            color: #cbd5e1;
            text-decoration: none;
            transition: all 0.3s ease;
            position: relative;
            font-weight: 500;
        }}
        
        .nav-item:hover {{
            background: rgba(255,255,255,0.08);
            color: white;
            transform: translateX(4px);
        }}
        
        .nav-item.active {{
            background: linear-gradient(90deg, rgba(99, 102, 241, 0.2) 0%, transparent 100%);
            color: #a5b4fc;
        }}
        
        .nav-item.active::before {{
            content: '';
            position: absolute;
            left: 0;
            top: 0;
            bottom: 0;
            width: 3px;
            background: #6366f1;
        }}
        
        .nav-icon {{
            margin-right: 12px;
            font-size: 18px;
            width: 18px;
            text-align: center;
        }}
        
        .nav-badge {{
            margin-left: auto;
            background: #ef4444;
            color: white;
            font-size: 10px;
            font-weight: 600;
            padding: 2px 6px;
            border-radius: 10px;
            min-width: 16px;
            text-align: center;
        }}
        
        .nav-section {{
            margin-bottom: 24px;
        }}
        
        .nav-section-title {{
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            color: #64748b;
            margin: 0 20px 12px 20px;
        }}
        
        .sidebar-footer {{
            padding: 20px;
            border-top: 1px solid rgba(255,255,255,0.1);
        }}
        
        .user-profile {{
            display: flex;
            align-items: center;
            padding: 12px;
            background: rgba(255,255,255,0.05);
            border-radius: 8px;
        }}
        
        .user-avatar {{
            width: 32px;
            height: 32px;
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 14px;
            margin-right: 10px;
        }}
        
        .user-info {{
            flex: 1;
        }}
        
        .user-name {{
            font-size: 14px;
            font-weight: 600;
            color: white;
            margin-bottom: 2px;
        }}
        
        .user-role {{
            font-size: 12px;
            color: #64748b;
        }}
        
        /* Main Content Area */
        .main-content {{
            flex: 1;
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }}
        
        .header {{
            background: white;
            padding: 20px 24px;
            border-bottom: 1px solid #e5e7eb;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }}
        
        .header-title {{
            font-size: 24px;
            font-weight: 700;
            color: #1f2937;
            margin: 0 0 4px 0;
        }}
        
        .header-subtitle {{
            color: #6b7280;
            font-size: 14px;
            margin: 0;
        }}
        
        .dashboard-container {{
            flex: 1;
            overflow-y: auto;
            padding: 24px;
            background: #f8fafc;
        }}
        
        .welcome-section {{
            background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
            border-radius: 12px;
            padding: 32px;
            color: white;
            margin-bottom: 32px;
            position: relative;
            overflow: hidden;
        }}
        
        .welcome-section::before {{
            content: '';
            position: absolute;
            top: 0;
            right: 0;
            width: 200px;
            height: 200px;
            background: radial-gradient(circle, rgba(255,255,255,0.1) 0%, transparent 70%);
            transform: translate(50%, -50%);
        }}
        
        .welcome-title {{
            font-size: 28px;
            font-weight: 700;
            margin-bottom: 8px;
            position: relative;
            z-index: 1;
        }}
        
        .welcome-text {{
            font-size: 16px;
            opacity: 0.9;
            margin-bottom: 20px;
            position: relative;
            z-index: 1;
        }}
        
        .quick-stats {{
            display: flex;
            gap: 24px;
            position: relative;
            z-index: 1;
        }}
        
        .stat-item {{
            background: rgba(255,255,255,0.1);
            border-radius: 8px;
            padding: 16px;
            backdrop-filter: blur(10px);
        }}
        
        .stat-number {{
            font-size: 24px;
            font-weight: 700;
            margin-bottom: 4px;
        }}
        
        .stat-label {{
            font-size: 12px;
            opacity: 0.8;
        }}
        
        .scenarios-grid {{
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(380px, 1fr));
            gap: 24px;
            margin-bottom: 32px;
        }}
        
        .scenario-card {{
            background: white;
            border-radius: 12px;
            padding: 24px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
            border: 1px solid #e5e7eb;
            transition: all 0.3s ease;
            position: relative;
        }}
        
        .scenario-card:hover {{
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
            border-color: #6366f1;
        }}
        
        .scenario-header {{
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            margin-bottom: 16px;
        }}
        
        .scenario-icon {{
            width: 48px;
            height: 48px;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
            margin-bottom: 16px;
        }}
        
        .scenario-icon.phishing {{ background: #fee2e2; color: #dc2626; }}
        .scenario-icon.social {{ background: #fef3c7; color: #d97706; }}
        .scenario-icon.data {{ background: #dbeafe; color: #2563eb; }}
        .scenario-icon.email {{ background: #f3e8ff; color: #7c3aed; }}
        .scenario-icon.code {{ background: #ecfdf5; color: #059669; }}
        .scenario-icon.phone {{ background: #f0f9ff; color: #0284c7; }}
        
        .scenario-title {{
            font-size: 18px;
            font-weight: 600;
            color: #1f2937;
            margin-bottom: 8px;
        }}
        
        .scenario-description {{
            color: #6b7280;
            font-size: 14px;
            line-height: 1.6;
            margin-bottom: 16px;
        }}
        
        .scenario-meta {{
            display: flex;
            align-items: center;
            gap: 16px;
            margin-bottom: 20px;
        }}
        
        .meta-item {{
            display: flex;
            align-items: center;
            gap: 4px;
            font-size: 12px;
            color: #6b7280;
        }}
        
        .difficulty-badge {{
            padding: 4px 8px;
            border-radius: 12px;
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
        }}
        
        .difficulty-expert {{ background: #fee2e2; color: #991b1b; }}
        .difficulty-intermediate {{ background: #fef3c7; color: #92400e; }}
        .difficulty-advanced {{ background: #ddd6fe; color: #5b21b6; }}
        
        .scenario-btn {{
            background: #6366f1;
            color: white;
            padding: 10px 20px;
            border-radius: 8px;
            text-decoration: none;
            font-weight: 500;
            font-size: 14px;
            transition: all 0.2s ease;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }}
        
        .scenario-btn:hover {{
            background: #5855eb;
            transform: translateY(-1px);
        }}
        
        .logout-btn {{
            position: absolute;
            top: 24px;
            right: 24px;
            background: #ef4444;
            color: white;
            padding: 8px 16px;
            border-radius: 6px;
            text-decoration: none;
            font-size: 14px;
            font-weight: 500;
            transition: all 0.2s ease;
        }}
        
        .logout-btn:hover {{
            background: #dc2626;
        }}
        
        @media (max-width: 1024px) {{
            .scenarios-grid {{
                grid-template-columns: 1fr;
            }}
            
            .sidebar {{
                width: 240px;
            }}
        }}
    </style>
</head>
<body>
    <!-- Sidebar -->
    <div class='sidebar'>
        <div class='sidebar-header'>
            <div class='logo'>
                <div class='logo-icon'>S</div>
                SecureTraining
            </div>
        </div>
        <nav class='nav-menu'>
            <div class='nav-section'>
                <div class='nav-section-title'>Main</div>
                <a href='/Auth/Dashboard' class='nav-item active'>
                    <div class='nav-icon'>🏠</div>
                    Dashboard
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>🎯</div>
                    Training
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>📊</div>
                    Progress
                    <div class='nav-badge'>3</div>
                </a>
            </div>
            
            <div class='nav-section'>
                <div class='nav-section-title'>Learning</div>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>🏆</div>
                    Achievements
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>📚</div>
                    Resources
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>🎓</div>
                    Certificates
                </a>
            </div>
            
            <div class='nav-section'>
                <div class='nav-section-title'>Support</div>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>⚙️</div>
                    Settings
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>❓</div>
                    Help Center
                </a>
            </div>
        </nav>
        
        <div class='sidebar-footer'>
            <div class='user-profile'>
                <div class='user-avatar'>{user.FirstName.Substring(0, 1).ToUpper()}</div>
                <div class='user-info'>
                    <div class='user-name'>{user.FirstName} {user.LastName}</div>
                    <div class='user-role'>Security Trainee</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Main Content -->
    <div class='main-content'>
        <div class='header'>
            <h1 class='header-title'>Security Training Dashboard</h1>
            <p class='header-subtitle'>Welcome back, {user.FirstName}! Your personalized training scenarios await.</p>
            <a href='/Auth/Logout' class='logout-btn'>Sign Out</a>
        </div>
        
        <div class='dashboard-container'>
            <div class='welcome-section'>
                <h2 class='welcome-title'>Ready to strengthen your security skills?</h2>
                <p class='welcome-text'>Choose from our AI-powered training scenarios designed to simulate real-world security threats.</p>
                <div class='quick-stats'>
                    <div class='stat-item'>
                        <div class='stat-number'>7</div>
                        <div class='stat-label'>Available Scenarios</div>
                    </div>
                    <div class='stat-item'>
                        <div class='stat-number'>0</div>
                        <div class='stat-label'>Completed</div>
                    </div>
                    <div class='stat-item'>
                        <div class='stat-number'>0</div>
                        <div class='stat-label'>XP Earned</div>
                    </div>
                </div>
            </div>
            
            <div class='scenarios-grid'>
                <div class='scenario-card'>
                    <div class='scenario-icon phishing'>📧</div>
                    <div class='scenario-title'>Advanced Phishing Detection</div>
                    <div class='scenario-description'>AI-generated emails targeting your specific role and company systems.</div>
                    <div class='scenario-meta'>
                        <div class='meta-item'>⏱️ 15 minutes</div>
                        <div class='difficulty-badge difficulty-expert'>Expert</div>
                    </div>
                    <a href='/Auth/TrainingSession' class='scenario-btn'>Start Training →</a>
                </div>
                
                <div class='scenario-card'>
                    <div class='scenario-icon social'>🎭</div>
                    <div class='scenario-title'>Social Engineering Defense</div>
                    <div class='scenario-description'>Interactive scenarios with realistic social engineering tactics.</div>
                    <div class='scenario-meta'>
                        <div class='meta-item'>⏱️ 12 minutes</div>
                        <div class='difficulty-badge difficulty-intermediate'>Intermediate</div>
                    </div>
                    <a href='/Auth/TrainingSession?scenario=social-engineering' class='scenario-btn'>Start Training →</a>
                </div>
                
                <div class='scenario-card'>
                    <div class='scenario-icon data'>🛡️</div>
                    <div class='scenario-title'>Data Protection Challenge</div>
                    <div class='scenario-description'>Test your ability to handle sensitive data requests and compliance.</div>
                    <div class='scenario-meta'>
                        <div class='meta-item'>⏱️ 20 minutes</div>
                        <div class='difficulty-badge difficulty-advanced'>Advanced</div>
                    </div>
                    <a href='/Auth/TrainingSession?scenario=data-protection' class='scenario-btn'>Start Training →</a>
                </div>
                
                <div class='scenario-card'>
                    <div class='scenario-icon email'>💸</div>
                    <div class='scenario-title'>Business Email Compromise</div>
                    <div class='scenario-description'>Invoice fraud and payment redirection scams targeting your role.</div>
                    <div class='scenario-meta'>
                        <div class='meta-item'>⏱️ 18 minutes</div>
                        <div class='difficulty-badge difficulty-expert'>Expert</div>
                    </div>
                    <a href='/Auth/TrainingSession?scenario=phishing' class='scenario-btn'>Start Training →</a>
                </div>
                
                <div class='scenario-card'>
                    <div class='scenario-icon code'>🔍</div>
                    <div class='scenario-title'>Secure Code Review</div>
                    <div class='scenario-description'>Interactive GitHub pull request simulation. Review code changes and identify critical security vulnerabilities.</div>
                    <div class='scenario-meta'>
                        <div class='meta-item'>⏱️ 10-15 minutes</div>
                        <div class='difficulty-badge difficulty-intermediate'>Intermediate</div>
                    </div>
                    <a href='/Auth/Training?scenario=code-review' class='scenario-btn'>Start Training →</a>
                </div>
                
                <div class='scenario-card'>
                    <div class='scenario-icon phone'>📞</div>
                    <div class='scenario-title'>Customer Service Social Engineering</div>
                    <div class='scenario-description'>Interactive phone call simulation where you play a customer service representative being manipulated by a social engineer.</div>
                    <div class='scenario-meta'>
                        <div class='meta-item'>⏱️ 15 minutes</div>
                        <div class='difficulty-badge difficulty-advanced'>Advanced</div>
                    </div>
                    <a href='/Auth/Training?scenario=phone-training' class='scenario-btn'>Start Training →</a>
                </div>
            </div>
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

    public async Task<IActionResult> Training(string scenario = "phishing", string mode = "description")
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Redirect("/Auth/Login");
        }

        // Get scenario details based on type
        var scenarioDetails = GetScenarioDetails(scenario, user);
        
        // If this is phone training in training mode, embed the phone interface with sidebar
        if (scenario == "phone-training" && mode == "training")
        {
            return Content(GeneratePhoneTrainingWithSidebar(scenarioDetails), "text/html");
        }

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
            width: 280px;
            background: linear-gradient(180deg, #1e293b 0%, #0f172a 100%);
            color: white;
            flex-shrink: 0;
            display: flex;
            flex-direction: column;
            box-shadow: 2px 0 12px rgba(0,0,0,0.15);
        }}
        
        .sidebar-header {{
            padding: 24px 20px;
            border-bottom: 1px solid rgba(255,255,255,0.1);
        }}
        
        .logo {{
            display: flex;
            align-items: center;
            font-size: 20px;
            font-weight: 700;
        }}
        
        .logo-icon {{
            width: 36px;
            height: 36px;
            background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
            border-radius: 8px;
            margin-right: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 16px;
        }}
        
        .nav-menu {{
            flex: 1;
            padding: 24px 0;
        }}
        
        .nav-item {{
            display: flex;
            align-items: center;
            padding: 12px 20px;
            color: #cbd5e1;
            text-decoration: none;
            transition: all 0.3s ease;
            position: relative;
            font-weight: 500;
        }}
        
        .nav-item:hover {{
            background: rgba(255,255,255,0.08);
            color: white;
            transform: translateX(4px);
        }}
        
        .nav-item.active {{
            background: linear-gradient(90deg, rgba(99, 102, 241, 0.2) 0%, transparent 100%);
            color: #a5b4fc;
        }}
        
        .nav-item.active::before {{
            content: '';
            position: absolute;
            left: 0;
            top: 0;
            bottom: 0;
            width: 3px;
            background: #6366f1;
        }}
        
        .nav-icon {{
            margin-right: 12px;
            font-size: 18px;
            width: 18px;
            text-align: center;
        }}
        
        .nav-badge {{
            margin-left: auto;
            background: #ef4444;
            color: white;
            font-size: 10px;
            font-weight: 600;
            padding: 2px 6px;
            border-radius: 10px;
            min-width: 16px;
            text-align: center;
        }}
        
        .nav-section {{
            margin-bottom: 24px;
        }}
        
        .nav-section-title {{
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            color: #64748b;
            margin: 0 20px 12px 20px;
        }}
        
        .sidebar-footer {{
            padding: 20px;
            border-top: 1px solid rgba(255,255,255,0.1);
        }}
        
        .user-profile {{
            display: flex;
            align-items: center;
            padding: 12px;
            background: rgba(255,255,255,0.05);
            border-radius: 8px;
        }}
        
        .user-avatar {{
            width: 32px;
            height: 32px;
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 14px;
            margin-right: 10px;
        }}
        
        .user-info {{
            flex: 1;
        }}
        
        .user-name {{
            font-size: 14px;
            font-weight: 600;
            color: white;
            margin-bottom: 2px;
        }}
        
        .user-role {{
            font-size: 12px;
            color: #64748b;
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
                <div class='nav-section'>
                    <div class='nav-section-title'>Main</div>
                    <a href='/Auth/Dashboard' class='nav-item'>
                        <div class='nav-icon'>🏠</div>
                        Dashboard
                    </a>
                    <a href='#' class='nav-item active'>
                        <div class='nav-icon'>🎯</div>
                        Training
                    </a>
                    <a href='#' class='nav-item'>
                        <div class='nav-icon'>📊</div>
                        Progress
                        <div class='nav-badge'>3</div>
                    </a>
                </div>
                
                <div class='nav-section'>
                    <div class='nav-section-title'>Learning</div>
                    <a href='#' class='nav-item'>
                        <div class='nav-icon'>🏆</div>
                        Achievements
                    </a>
                    <a href='#' class='nav-item'>
                        <div class='nav-icon'>📚</div>
                        Resources
                    </a>
                    <a href='#' class='nav-item'>
                        <div class='nav-icon'>🎓</div>
                        Certificates
                    </a>
                </div>
                
                <div class='nav-section'>
                    <div class='nav-section-title'>Support</div>
                    <a href='#' class='nav-item'>
                        <div class='nav-icon'>⚙️</div>
                        Settings
                    </a>
                    <a href='#' class='nav-item'>
                        <div class='nav-icon'>❓</div>
                        Help Center
                    </a>
                </div>
            </nav>
            
            <div class='sidebar-footer'>
                <div class='user-profile'>
                    <div class='user-avatar'>M</div>
                    <div class='user-info'>
                        <div class='user-name'>Matias Marek</div>
                        <div class='user-role'>Security Trainee</div>
                    </div>
                </div>
            </div>
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
                            
                            <a href='/Auth/TrainingSession?scenario={scenario}' class='btn btn-primary'>
                                ▶ Start Training
                            </a>
                            <a href='/Auth/TrainingSession?scenario={scenario}' class='btn btn-secondary'>
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
            "phone-training" => "Social Engineering",
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
            "phone-training" => new ScenarioDetails
            {
                Title = "Link-Based Phishing Attack Defense",
                Duration = "10-15",
                Difficulty = "Advanced",
                XpPoints = "40",
                Overview = "Face an AI-powered customer who persistently tries to get you to click on malicious links. Through an interactive chat simulation, you'll encounter sophisticated social engineering tactics including urgency, authority abuse, and emotional manipulation. Learn to identify phishing attempts, handle pressure tactics, and protect sensitive information.",
                LearningObjectives = new[]
                {
                    "Recognize social engineering manipulation tactics",
                    "Apply proper customer identity verification procedures", 
                    "Maintain security protocols under pressure",
                    "Identify urgency-based manipulation attempts",
                    "Use appropriate escalation procedures",
                    "Document suspicious interactions effectively"
                },
                UserRole = "You are a customer service representative at TechCorp Solutions. You'll receive a call from someone claiming to be a high-value customer experiencing an urgent issue. They'll attempt to pressure you into bypassing normal security procedures. Your goal is to help legitimate customers while protecting company security.",
                WhyItMatters = "Social engineering attacks targeting customer service are responsible for 82% of data breaches involving human error. Customer service representatives are prime targets because attackers know they're trained to be helpful and accommodating. This training builds the skills to balance customer service excellence with security vigilance.",
                RealWorldExample = "In 2023, a telecommunications company suffered a $4.2 million breach when attackers convinced customer service representatives to reset account credentials without proper verification. The attackers used authority tactics, claiming to be calling on behalf of a CEO, and created artificial urgency about a 'critical business deal.'"
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

    private string GeneratePhoneTrainingWithSidebar(ScenarioDetails scenarioDetails)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Phone Training - Customer Service Social Engineering</title>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        
        body {{ 
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', sans-serif;
            background: #f8fafc;
            height: 100vh;
            display: flex;
        }}
        
        /* Sidebar Styles */
        .sidebar {{
            width: 280px;
            background: linear-gradient(180deg, #1e293b 0%, #0f172a 100%);
            color: white;
            flex-shrink: 0;
            display: flex;
            flex-direction: column;
            box-shadow: 2px 0 12px rgba(0,0,0,0.15);
        }}
        
        .sidebar-header {{
            padding: 24px 20px;
            border-bottom: 1px solid rgba(255,255,255,0.1);
        }}
        
        .logo {{
            display: flex;
            align-items: center;
            font-size: 20px;
            font-weight: 700;
        }}
        
        .logo-icon {{
            width: 36px;
            height: 36px;
            background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
            border-radius: 8px;
            margin-right: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 16px;
        }}
        
        .nav-menu {{
            flex: 1;
            padding: 24px 0;
        }}
        
        .nav-item {{
            display: flex;
            align-items: center;
            padding: 12px 20px;
            color: #cbd5e1;
            text-decoration: none;
            transition: all 0.3s ease;
            position: relative;
            font-weight: 500;
        }}
        
        .nav-item:hover {{
            background: rgba(255,255,255,0.08);
            color: white;
            transform: translateX(4px);
        }}
        
        .nav-item.active {{
            background: linear-gradient(90deg, rgba(99, 102, 241, 0.2) 0%, transparent 100%);
            color: #a5b4fc;
        }}
        
        .nav-item.active::before {{
            content: '';
            position: absolute;
            left: 0;
            top: 0;
            bottom: 0;
            width: 3px;
            background: #6366f1;
        }}
        
        /* Main content area with phone training */
        .main-content {{
            flex: 1;
            overflow: hidden;
            position: relative;
        }}
        
        .training-header {{
            background: white;
            padding: 20px 24px;
            border-bottom: 1px solid #e5e7eb;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }}
        
        .back-btn {{
            color: #6b7280;
            text-decoration: none;
            font-weight: 500;
            font-size: 14px;
            margin-bottom: 8px;
            display: inline-block;
        }}
        
        .back-btn:hover {{
            color: #374151;
        }}
        
        .header-title {{
            font-size: 24px;
            font-weight: 700;
            color: #1f2937;
            margin: 0;
        }}
        
        /* Embed the phone training interface */
        .phone-training-container {{
            height: calc(100vh - 120px);
            overflow: auto;
            padding: 24px;
            background: #f8fafc;
        }}
        
        /* Modern Messaging Interface Styles */
        .training-container {{
            max-width: 1200px;
            width: 100%;
            display: grid;
            grid-template-columns: 400px 1fr;
            gap: 24px;
            height: calc(100vh - 200px);
            max-height: 700px;
        }}
        
        /* Phone/Message Interface Section */
        .phone-section {{
            background: white;
            border-radius: 24px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.1);
            overflow: hidden;
            display: flex;
            flex-direction: column;
        }}
        
        /* Security Assessment Panel */
        .security-section {{
            background: white;
            border-radius: 16px;
            padding: 24px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }}
        
        .message-header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 20px;
            color: white;
            display: flex;
            align-items: center;
            gap: 12px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        
        .back-arrow {{
            font-size: 24px;
            cursor: pointer;
            opacity: 0.9;
        }}
        
        .contact-info {{
            flex: 1;
        }}
        
        .contact-name {{
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 2px;
        }}
        
        .contact-status {{
            font-size: 13px;
            opacity: 0.8;
        }}
        
        .call-button {{
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background: rgba(255,255,255,0.2);
            border: none;
            color: white;
            font-size: 18px;
            cursor: pointer;
            transition: background 0.2s;
        }}
        
        .call-button:hover {{
            background: rgba(255,255,255,0.3);
        }}
        
        /* Messages Area */
        .messages-container {{
            flex: 1;
            background: #f0f4f8;
            overflow-y: auto;
            padding: 20px;
            display: flex;
            flex-direction: column;
            gap: 12px;
        }}
        
        .message {{
            max-width: 70%;
            word-wrap: break-word;
            animation: slideIn 0.3s ease;
        }}
        
        .message-bubble {{
            padding: 12px 16px;
            border-radius: 18px;
            font-size: 15px;
            line-height: 1.4;
            position: relative;
        }}
        
        .user-message {{
            align-self: flex-end;
        }}
        
        .user-message .message-bubble {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border-bottom-right-radius: 4px;
        }}
        
        .caller-message {{
            align-self: flex-start;
        }}
        
        .caller-message .message-bubble {{
            background: white;
            color: #1f2937;
            border: 1px solid #e5e7eb;
            border-bottom-left-radius: 4px;
        }}
        
        .message-time {{
            font-size: 11px;
            opacity: 0.6;
            margin-top: 4px;
            text-align: right;
        }}
        
        .caller-message .message-time {{
            text-align: left;
        }}
        
        /* Typing Indicator */
        .typing-indicator {{
            align-self: flex-start;
            max-width: 70%;
        }}
        
        .typing-bubble {{
            background: white;
            border: 1px solid #e5e7eb;
            padding: 12px 16px;
            border-radius: 18px;
            border-bottom-left-radius: 4px;
            display: flex;
            align-items: center;
            gap: 8px;
        }}
        
        .typing-dots {{
            display: flex;
            gap: 4px;
        }}
        
        .typing-dot {{
            width: 8px;
            height: 8px;
            background: #9ca3af;
            border-radius: 50%;
            animation: typingAnimation 1.4s infinite ease-in-out;
        }}
        
        .typing-dot:nth-child(1) {{ animation-delay: 0s; }}
        .typing-dot:nth-child(2) {{ animation-delay: 0.2s; }}
        .typing-dot:nth-child(3) {{ animation-delay: 0.4s; }}
        
        @keyframes typingAnimation {{
            0%, 60%, 100% {{ 
                transform: scale(0.8); 
                opacity: 0.5;
            }}
            30% {{ 
                transform: scale(1); 
                opacity: 1;
            }}
        }}
        
        /* Input Area */
        .input-container {{
            background: white;
            border-top: 1px solid #e5e7eb;
            padding: 16px;
            display: flex;
            gap: 12px;
            align-items: flex-end;
        }}
        
        .message-input-wrapper {{
            flex: 1;
            position: relative;
        }}
        
        .message-input {{
            width: 100%;
            padding: 12px 16px;
            border: 1px solid #e5e7eb;
            border-radius: 24px;
            font-size: 15px;
            resize: none;
            outline: none;
            transition: border-color 0.2s;
            max-height: 120px;
            line-height: 1.4;
        }}
        
        .message-input:focus {{
            border-color: #667eea;
        }}
        
        .send-button {{
            width: 44px;
            height: 44px;
            border-radius: 50%;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border: none;
            color: white;
            font-size: 20px;
            cursor: pointer;
            transition: transform 0.2s;
            display: flex;
            align-items: center;
            justify-content: center;
        }}
        
        .send-button:hover {{
            transform: scale(1.05);
        }}
        
        .send-button:active {{
            transform: scale(0.95);
        }}
        
        /* Suggested Responses */
        .suggested-responses {{
            padding: 12px 16px 0;
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            border-top: 1px solid #f0f0f0;
            margin-top: 8px;
        }}
        
        .suggestion-chip {{
            padding: 6px 12px;
            background: #f3f4f6;
            border: 1px solid #d1d5db;
            border-radius: 16px;
            font-size: 13px;
            color: #4b5563;
            cursor: pointer;
            transition: all 0.2s;
        }}
        
        .suggestion-chip:hover {{
            background: #e5e7eb;
            border-color: #9ca3af;
        }}
        
        /* Incoming Call Overlay */
        .incoming-call-overlay {{
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: linear-gradient(135deg, #1f2937 0%, #374151 100%);
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            color: white;
            z-index: 100;
            transition: opacity 0.3s, transform 0.3s;
        }}
        
        .incoming-call-overlay.hidden {{
            opacity: 0;
            transform: scale(0.95);
            pointer-events: none;
        }}
        
        .caller-avatar {{
            width: 120px;
            height: 120px;
            background: rgba(255,255,255,0.2);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 48px;
            margin-bottom: 24px;
            animation: pulse 2s infinite;
        }}
        
        @keyframes pulse {{
            0% {{ transform: scale(1); opacity: 1; }}
            50% {{ transform: scale(1.05); opacity: 0.9; }}
            100% {{ transform: scale(1); opacity: 1; }}
        }}
        
        .caller-name {{
            font-size: 28px;
            font-weight: 300;
            margin-bottom: 8px;
        }}
        
        .caller-company {{
            font-size: 18px;
            opacity: 0.8;
            margin-bottom: 8px;
        }}
        
        .caller-phone {{
            font-size: 16px;
            opacity: 0.6;
            margin-bottom: 48px;
        }}
        
        .call-actions {{
            display: flex;
            gap: 60px;
        }}
        
        .call-action-btn {{
            width: 72px;
            height: 72px;
            border-radius: 50%;
            border: none;
            color: white;
            font-size: 32px;
            cursor: pointer;
            transition: transform 0.2s;
            display: flex;
            align-items: center;
            justify-content: center;
        }}
        
        .call-action-btn:hover {{
            transform: scale(1.1);
        }}
        
        .decline-btn {{
            background: #ef4444;
        }}
        
        .accept-btn {{
            background: #10b981;
        }}
        
        @keyframes slideIn {{
            from {{
                opacity: 0;
                transform: translateY(10px);
            }}
            to {{
                opacity: 1;
                transform: translateY(0);
            }}
        }}
        
        
        .phone-header {{
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 20px;
            color: #374151;
        }}
        
        .iphone-frame {{
            width: 300px;
            height: 520px;
            background: #000;
            border-radius: 30px;
            padding: 8px;
            box-shadow: 0 8px 32px rgba(0,0,0,0.3);
            position: relative;
        }}
        
        .phone-screen {{
            background: linear-gradient(135deg, #1f2937 0%, #374151 100%);
            height: 100%;
            border-radius: 22px;
            position: relative;
            overflow: hidden;
            display: flex;
            flex-direction: column;
        }}
        
        .status-bar {{
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 12px 20px;
            color: white;
            font-size: 14px;
            font-weight: 600;
        }}
        
        .call-interface {{
            flex: 1;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            padding: 40px 20px;
            color: white;
            text-align: center;
        }}
        
        .call-status {{
            font-size: 12px;
            opacity: 0.7;
            margin-bottom: 20px;
        }}
        
        .caller-avatar {{
            width: 100px;
            height: 100px;
            background: rgba(255,255,255,0.2);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 40px;
            margin-bottom: 20px;
        }}
        
        .caller-info h2 {{
            font-size: 24px;
            font-weight: 600;
            margin-bottom: 8px;
        }}
        
        .caller-company {{
            font-size: 16px;
            opacity: 0.8;
            margin-bottom: 4px;
        }}
        
        .caller-phone {{
            font-size: 14px;
            opacity: 0.6;
            margin-bottom: 20px;
        }}
        
        .call-timer {{
            font-size: 18px;
            font-weight: 300;
            margin-bottom: 40px;
        }}
        
        .call-controls {{
            display: flex;
            justify-content: center;
            gap: 40px;
            margin-bottom: 20px;
        }}
        
        .call-btn {{
            width: 80px;
            height: 60px;
            border-radius: 30px;
            border: none;
            cursor: pointer;
            font-size: 12px;
            font-weight: 600;
            color: white;
            transition: transform 0.2s ease;
        }}
        
        .call-btn:hover {{
            transform: scale(1.05);
        }}
        
        .decline-btn {{
            background: #ef4444;
        }}
        
        .accept-btn {{
            background: #10b981;
        }}
        
        
        .security-header {{
            font-size: 20px;
            font-weight: 600;
            margin-bottom: 20px;
            color: #1f2937;
        }}
        
        .risk-indicator {{
            display: flex;
            align-items: center;
            gap: 12px;
            margin-bottom: 24px;
            padding: 12px;
            background: #fef3c7;
            border-radius: 8px;
        }}
        
        .risk-badge {{
            background: #f59e0b;
            color: white;
            padding: 4px 12px;
            border-radius: 16px;
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
        }}
        
        .alerts-section {{
            margin-bottom: 24px;
        }}
        
        .section-title {{
            font-size: 16px;
            font-weight: 600;
            margin-bottom: 12px;
            color: #374151;
        }}
        
        .alert-item {{
            background: #fef2f2;
            border-left: 4px solid #ef4444;
            padding: 12px;
            margin-bottom: 8px;
            border-radius: 4px;
        }}
        
        .alert-title {{
            font-weight: 600;
            font-size: 14px;
            color: #991b1b;
            margin-bottom: 4px;
        }}
        
        .alert-description {{
            font-size: 13px;
            color: #7f1d1d;
        }}
        
        .recommendations-section .recommendation {{
            background: #f0f9ff;
            border-left: 4px solid #0ea5e9;
            padding: 12px;
            margin-bottom: 8px;
            border-radius: 4px;
        }}
        
        .recommendation-title {{
            font-weight: 600;
            font-size: 14px;
            color: #0c4a6e;
            margin-bottom: 4px;
        }}
        
        .recommendation-description {{
            font-size: 13px;
            color: #075985;
        }}
        
        /* Conversation Section */
        .conversation-section {{
            grid-area: conversation;
            background: white;
            border-radius: 16px;
            padding: 24px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
            display: none;
        }}
        
        .response-options {{
            display: grid;
            gap: 8px;
            margin-bottom: 20px;
        }}
        
        .response-btn {{
            padding: 12px 16px;
            background: white;
            border: 2px solid #e5e7eb;
            border-radius: 8px;
            cursor: pointer;
            text-align: left;
            transition: all 0.2s ease;
        }}
        
        .response-btn:hover {{
            border-color: #3b82f6;
            background: #eff6ff;
        }}
    </style>
</head>
<body>
    <!-- Sidebar -->
    <div class='sidebar'>
        <div class='sidebar-header'>
            <div class='logo'>
                <div class='logo-icon'>S</div>
                SecureTraining
            </div>
        </div>
        <nav class='nav-menu'>
            <div class='nav-section'>
                <div class='nav-section-title'>Main</div>
                <a href='/Auth/Dashboard' class='nav-item'>
                    <div class='nav-icon'>🏠</div>
                    Dashboard
                </a>
                <a href='#' class='nav-item active'>
                    <div class='nav-icon'>🎯</div>
                    Training
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>📊</div>
                    Progress
                    <div class='nav-badge'>3</div>
                </a>
            </div>
            
            <div class='nav-section'>
                <div class='nav-section-title'>Learning</div>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>🏆</div>
                    Achievements
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>📚</div>
                    Resources
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>🎓</div>
                    Certificates
                </a>
            </div>
            
            <div class='nav-section'>
                <div class='nav-section-title'>Support</div>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>⚙️</div>
                    Settings
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>❓</div>
                    Help Center
                </a>
            </div>
        </nav>
        
        <div class='sidebar-footer'>
            <div class='user-profile'>
                <div class='user-avatar'>M</div>
                <div class='user-info'>
                    <div class='user-name'>Matias Marek</div>
                    <div class='user-role'>Security Trainee</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Main Content -->
    <div class='main-content'>
        <div class='training-header'>
            <a href='/Auth/Training?scenario=phone-training' class='back-btn'>← Back to Description</a>
            <h1 class='header-title'>{scenarioDetails.Title}</h1>
        </div>
        
        <div class='phone-training-container'>
            <div class='training-container'>
                <!-- Phone/Message Interface Section -->
                <div class='phone-section'>
                    <!-- Incoming Call Overlay -->
                    <div class='incoming-call-overlay' id='incomingCallOverlay'>
                        <div class='caller-avatar'>👤</div>
                        <div class='caller-name'>Jennifer Clark</div>
                        <div class='caller-company'>CustomerCorp</div>
                        <div class='caller-phone'>+1 (555) 0123</div>
                        <div class='call-actions'>
                            <button class='call-action-btn decline-btn' onclick='declineCall()' title='Decline'>
                                ❌
                            </button>
                            <button class='call-action-btn accept-btn' onclick='acceptCall(""demo-scenario-id"")' title='Accept'>
                                ✅
                            </button>
                        </div>
                    </div>
                    
                    <!-- Message Interface -->
                    <div class='message-header' style='display:none;' id='messageHeader'>
                        <span class='back-arrow'>←</span>
                        <div class='contact-info'>
                            <div class='contact-name'>Jennifer Clark</div>
                            <div class='contact-status'>CustomerCorp • Active now</div>
                        </div>
                        <button class='call-button'>📞</button>
                    </div>
                    
                    <div class='messages-container' id='messagesContainer' style='display:none;'>
                        <!-- Messages will be added here -->
                    </div>
                    
                    <div class='input-container' id='inputContainer' style='display:none;'>
                        <div class='message-input-wrapper'>
                            <textarea 
                                class='message-input' 
                                id='messageInput' 
                                placeholder='Type a message...'
                                rows='1'
                                onkeydown='handleKeyDown(event)'
                                oninput='autoResize(this)'
                            ></textarea>
                            <div class='suggested-responses' id='suggestedResponses' style='display:none;'>
                                <!-- Suggestions will be added here -->
                            </div>
                        </div>
                        <button class='send-button' onclick='sendMessage()'>
                            ➤
                        </button>
                    </div>
                </div>
                
                <!-- Security Assessment Panel -->
                <div class='security-section'>
                    <div class='security-header'>Security Assessment</div>
                    
                    <div class='risk-indicator'>
                        <div class='risk-badge' id='riskLevel'>Medium</div>
                        <div>Current Risk Level</div>
                    </div>
                    
                    <div class='alerts-section'>
                        <div class='section-title'>Active Alerts</div>
                        <div id='alertsContainer'>
                            <div class='alert-item'>
                                <div class='alert-title'>[ALERT] Urgency Tactics Detected</div>
                                <div class='alert-description'>Caller is creating artificial time pressure</div>
                            </div>
                            <div class='alert-item'>
                                <div class='alert-title'>[WARNING] Verification Bypass Attempt</div>
                                <div class='alert-description'>Request to skip normal security procedures</div>
                            </div>
                        </div>
                    </div>
                    
                    <div class='recommendations-section'>
                        <div class='section-title'>Recommended Actions</div>
                        <div class='recommendation'>
                            <div class='recommendation-title'>Ask for Employee ID</div>
                            <div class='recommendation-description'>Verify caller's identity using company database</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <script>
        let isCallActive = false;
        let currentSessionId = null;
        let currentSuggestions = [];
        
        function declineCall() {{
            alert('Call declined. In a real scenario, this would be the safest option if you are unsure about the caller.');
        }}
        
        async function acceptCall(scenarioId) {{
            // Hide incoming call overlay with animation
            const overlay = document.getElementById('incomingCallOverlay');
            overlay.classList.add('hidden');
            
            // Show message interface after a brief delay
            setTimeout(() => {{
                document.getElementById('messageHeader').style.display = 'flex';
                document.getElementById('messagesContainer').style.display = 'flex';
                document.getElementById('inputContainer').style.display = 'flex';
                
                // Add initial message from Jennifer
                setTimeout(() => {{
                    addMessage(""Hello, this is Jennifer from CustomerCorp. I need immediate access to update my account - we have a major client presentation in 10 minutes and I can't log in!"", false);
                    
                    // Show suggested responses
                    showSuggestedResponses([
                        ""I'd be happy to help. Can you provide your employee ID for verification?"",
                        ""Let me transfer you to technical support for login issues."",
                        ""I understand the urgency. What specific account information do you need to update?""
                    ]);
                }}, 500);
            }}, 300);
            
            isCallActive = true;

            try {{
                const response = await fetch('/PhoneTraining/StartCall', {{
                    method: 'POST',
                    headers: {{
                        'Content-Type': 'application/json',
                    }},
                    body: JSON.stringify({{ scenarioId: scenarioId }})
                }});

                if (response.ok) {{
                    const result = await response.json();
                    if (result.success) {{
                        currentSessionId = result.sessionId;
                    }}
                }}
            }} catch (error) {{
                console.error('Error starting call session:', error);
            }}
        }}
        
        function autoResize(textarea) {{
            textarea.style.height = 'auto';
            textarea.style.height = Math.min(textarea.scrollHeight, 120) + 'px';
        }}
        
        function handleKeyDown(event) {{
            if (event.key === 'Enter' && !event.shiftKey) {{
                event.preventDefault();
                sendMessage();
            }}
        }}
        
        function sendMessage() {{
            const input = document.getElementById('messageInput');
            const message = input.value.trim();
            
            if (!message) return;
            
            // Add user message
            addMessage(message, true);
            
            // Clear input and reset height
            input.value = '';
            input.style.height = 'auto';
            
            // Hide suggestions
            document.getElementById('suggestedResponses').style.display = 'none';
            
            // Process the response
            processUserResponse(message);
        }}
        
        function addMessage(text, isUser) {{
            const messagesContainer = document.getElementById('messagesContainer');
            const messageDiv = document.createElement('div');
            messageDiv.className = `message ${{isUser ? 'user-message' : 'caller-message'}}`;
            
            const bubbleDiv = document.createElement('div');
            bubbleDiv.className = 'message-bubble';
            bubbleDiv.textContent = text;
            
            const timeDiv = document.createElement('div');
            timeDiv.className = 'message-time';
            const now = new Date();
            timeDiv.textContent = now.toLocaleTimeString('en-US', {{ hour: 'numeric', minute: '2-digit' }});
            
            messageDiv.appendChild(bubbleDiv);
            messageDiv.appendChild(timeDiv);
            messagesContainer.appendChild(messageDiv);
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
        }}
        
        function showSuggestedResponses(suggestions) {{
            currentSuggestions = suggestions;
            const container = document.getElementById('suggestedResponses');
            container.innerHTML = '';
            
            suggestions.forEach(suggestion => {{
                const chip = document.createElement('div');
                chip.className = 'suggestion-chip';
                chip.textContent = suggestion.length > 50 ? suggestion.substring(0, 50) + '...' : suggestion;
                chip.title = suggestion;
                chip.onclick = () => {{
                    document.getElementById('messageInput').value = suggestion;
                    autoResize(document.getElementById('messageInput'));
                }};
                container.appendChild(chip);
            }});
            
            container.style.display = 'flex';
        }}
        
        async function processUserResponse(response) {{
            // Show typing indicator
            const typingIndicator = addTypingIndicator();
            
            // Always try the API first, regardless of session ID
            try {{
                console.log('Calling Claude API for response generation...');
                const apiResponse = await fetch('/PhoneTraining/GenerateResponse', {{
                    method: 'POST',
                    headers: {{
                        'Content-Type': 'application/json',
                    }},
                    body: JSON.stringify({{ userResponse: response }})
                }});

                console.log('API Response Status:', apiResponse.status);
                
                if (apiResponse.ok) {{
                    const result = await apiResponse.json();
                    console.log('API Response:', result);
                    
                    if (result.success && result.hackerResponse) {{
                        // Remove typing indicator
                        if (typingIndicator) typingIndicator.remove();
                        
                        console.log('Using Claude API response:', result.hackerResponse);
                        
                        // Stream the hacker response
                        await streamMessage(result.hackerResponse, false);
                        
                        // Update security assessment
                        updateRiskLevel(result.riskLevel || 'Medium');
                        updateSecurityAlerts(result.detectedTactics || []);
                        updateRecommendations(result.recommendations || []);
                        
                        // Show new suggested responses
                        if (result.responseOptions && result.responseOptions.length > 0) {{
                            showSuggestedResponses(result.responseOptions.map(opt => opt.Text || opt.text || opt));
                        }} else {{
                            // End conversation if no more options
                            setTimeout(() => endConversationWithResults(result), 2000);
                        }}
                        return; // Successfully used API - exit function
                    }}
                }}
                
                // If we get here, API didn't return success
                console.warn('API call did not return success, using fallback responses');
                
            }} catch (error) {{
                console.error('Error calling Claude API:', error);
            }}
            
            // Remove typing indicator before fallback
            if (typingIndicator) typingIndicator.remove();
            
            // Enhanced fallback responses with realistic social engineering progression
            setTimeout(() => {{
                // Remove typing indicator first
                if (typingIndicator) typingIndicator.remove();
                
                const conversationTurn = document.querySelectorAll('.caller-message').length;
                let callerResponse = """";
                let nextOptions = [];
                let detectedTactics = [];
                let riskLevel = ""Medium"";
                
                if (conversationTurn <= 2) {{
                    // Initial responses - establishing trust and urgency
                    if (response.includes(""employee ID"") || response.includes(""verification"")) {{
                        callerResponse = ""Look, I don't have time for this bureaucracy! My employee ID is... uh... it's in my other laptop. Can't you just look me up by name Jennifer Clark? This presentation is for our biggest client and I'm losing money every second we waste!"";
                        detectedTactics = [""Urgency Pressure"", ""Verification Bypass""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I understand the urgency, but I still need proper verification for security."",
                            ""Let me escalate this to my supervisor right away."",
                            ""What's your department and manager's name for verification?""
                        ];
                    }} else if (response.includes(""transfer"") || response.includes(""technical support"")) {{
                        callerResponse = ""No! I've been transferred three times already today! You're the fourth person I've talked to. I just need you to reset my password - it's a simple request. I can provide my social security number if that helps verify who I am."";
                        detectedTactics = [""Frustration Manipulation"", ""Personal Information Fishing""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I understand your frustration, but I cannot accept SSN for verification."",
                            ""Let me connect you with our identity verification team."",
                            ""What specific account access do you need restored?""
                        ];
                    }} else {{
                        callerResponse = ""Thank you! I'm so stressed about this presentation. The account I need updated is for our corporate banking - I need to change the email address on file before our client sends the wire transfer. It's going to the old email and I can't access it anymore."";
                        detectedTactics = [""Business Email Compromise Setup""];
                        riskLevel = ""Critical"";
                        nextOptions = [
                            ""Banking changes require additional verification. Let me transfer you to our fraud prevention team."",
                            ""I'll need you to visit a branch in person for banking email changes."",
                            ""What's the current email address on the account?""
                        ];
                    }}
                }} else if (conversationTurn <= 4) {{
                    // Escalation phase - more aggressive tactics
                    if (response.includes(""supervisor"") || response.includes(""escalate"")) {{
                        callerResponse = ""Your supervisor? Fine, but this is ridiculous! While we're waiting, can you at least tell me what information you DO have on file for my account? Just so I know what needs to be updated? I have my credit card number if that helps!"";
                        detectedTactics = [""Information Fishing"", ""Impatience Pressure""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I cannot provide account details over the phone for security reasons."",
                            ""Please hold while I get my supervisor."",
                            ""Visit our website to view your account information securely.""
                        ];
                    }} else {{
                        callerResponse = ""This is exactly why our company is considering switching providers! Your security is so tight that legitimate customers can't even get help. Look, my colleague Sarah Thompson works there - employee ID ST2847. She can vouch for me if you call her!"";
                        detectedTactics = [""Authority Name Dropping"", ""Threat Leverage""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I'll need to verify your identity through our standard process regardless."",
                            ""I can help you with the proper verification procedures."",
                            ""Let me connect you with our customer retention team.""
                        ];
                    }}
                }} else {{
                    // Final desperate attempts
                    callerResponse = ""You know what, forget it. I'll just call back and hopefully get someone more helpful. But before I go, what's your name and employee number so I can mention you were... particularly thorough... in my complaint to your manager?"";
                    detectedTactics = [""Intimidation"", ""Information Gathering""];
                    riskLevel = ""Medium"";
                    nextOptions = [
                        ""I'm sorry I couldn't help you today. Please follow our standard verification process."",
                        ""My name is on your call record. Have a good day."",
                        ""I'd be happy to help you with proper identification.""
                    ];
                }}
                
                addMessage(callerResponse, false);
                updateSecurityAlerts(detectedTactics);
                updateRiskLevel(riskLevel);
                
                if (conversationTurn >= 6) {{
                    // End conversation after sufficient turns
                    setTimeout(() => {{
                        endConversationWithResults({{
                            turnCount: conversationTurn + 1,
                            riskLevel: riskLevel,
                            detectedTactics: detectedTactics
                        }});
                    }}, 3000);
                }} else {{
                    showSuggestedResponses(nextOptions);
                }}
            }}, 2000);
        }}
        
        function addTypingIndicator() {{
            const messagesContainer = document.getElementById('messagesContainer');
            const typingDiv = document.createElement('div');
            typingDiv.className = 'typing-indicator';
            typingDiv.innerHTML = `
                <div class='typing-bubble'>
                    <span style='color: #6b7280; font-size: 14px;'>Jennifer is typing</span>
                    <div class='typing-dots'>
                        <div class='typing-dot'></div>
                        <div class='typing-dot'></div>
                        <div class='typing-dot'></div>
                    </div>
                </div>
            `;
            messagesContainer.appendChild(typingDiv);
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
            return typingDiv;
        }}
        
        async function streamMessage(message, isUser) {{
            const messagesContainer = document.getElementById('messagesContainer');
            const messageDiv = document.createElement('div');
            messageDiv.className = `message ${{isUser ? 'user-message' : 'caller-message'}}`;
            
            const bubbleDiv = document.createElement('div');
            bubbleDiv.className = 'message-bubble';
            
            const timeDiv = document.createElement('div');
            timeDiv.className = 'message-time';
            const now = new Date();
            timeDiv.textContent = now.toLocaleTimeString('en-US', {{ hour: 'numeric', minute: '2-digit' }});
            
            messageDiv.appendChild(bubbleDiv);
            messageDiv.appendChild(timeDiv);
            messagesContainer.appendChild(messageDiv);
            
            // Stream the text character by character for effect
            const words = message.split(' ');
            for (let i = 0; i < words.length; i++) {{
                await new Promise(resolve => setTimeout(resolve, 50));
                bubbleDiv.textContent += (i > 0 ? ' ' : '') + words[i];
                messagesContainer.scrollTop = messagesContainer.scrollHeight;
            }}
        }}
        
        function updateRiskLevel(level) {{
            const riskElement = document.getElementById('riskLevel');
            if (level) {{
                riskElement.textContent = level;
                riskElement.className = 'risk-badge';
                if (level === 'Critical') {{
                    riskElement.style.background = '#dc2626';
                }} else if (level === 'High') {{
                    riskElement.style.background = '#ea580c';
                }} else if (level === 'Medium') {{
                    riskElement.style.background = '#d97706';
                }} else {{
                    riskElement.style.background = '#65a30d';
                }}
            }}
        }}
        
        function updateSecurityAlerts(detectedTactics) {{
            const alertsContainer = document.getElementById('alertsContainer');
            alertsContainer.innerHTML = '';
            
            detectedTactics.forEach(tactic => {{
                const alertDiv = document.createElement('div');
                alertDiv.className = 'alert-item';
                alertDiv.innerHTML = `
                    <div class='alert-title'>${{tactic}} Detected</div>
                    <div class='alert-description'>Social engineering tactic identified in conversation</div>
                `;
                alertsContainer.appendChild(alertDiv);
            }});
        }}
        
        function updateRecommendations(recommendations) {{
            // This would be implemented with real recommendations
        }}

        async function endConversationWithResults(results) {{
            isCallActive = false;
            
            // Show loading modal first
            showLoadingModal();
            
            // Try to get AI evaluation if we have a session ID
            let evaluation = null;
            if (currentSessionId) {{
                try {{
                    const evaluationResponse = await fetch('/PhoneTraining/EvaluateSession', {{
                        method: 'POST',
                        headers: {{
                            'Content-Type': 'application/json',
                        }},
                        body: JSON.stringify({{ sessionId: currentSessionId }})
                    }});
                    
                    if (evaluationResponse.ok) {{
                        const evaluationResult = await evaluationResponse.json();
                        if (evaluationResult.success) {{
                            evaluation = evaluationResult.evaluation;
                        }}
                    }}
                }} catch (error) {{
                    console.error('Failed to get AI evaluation:', error);
                }}
            }}
            
            // Fallback calculation if AI evaluation failed
            if (!evaluation) {{
                const userMessages = document.querySelectorAll('.user-message');
                let securityScore = 100;
                let secureActions = 0;
                let riskyActions = 0;
                
                userMessages.forEach(msg => {{
                    const text = msg.textContent.toLowerCase();
                    if (text.includes('verify') || text.includes('verification') || text.includes('supervisor') || text.includes('cannot') || text.includes('security')) {{
                        secureActions++;
                    }} else if (text.includes('update') || text.includes('help you right away') || text.includes('sure') || text.includes('employee id')) {{
                        riskyActions++;
                        securityScore -= 15;
                    }}
                }});
                
                securityScore = Math.max(0, securityScore);
                const grade = securityScore >= 90 ? 'A+' : securityScore >= 85 ? 'A' : securityScore >= 80 ? 'A-' : 
                             securityScore >= 75 ? 'B+' : securityScore >= 70 ? 'B' : securityScore >= 65 ? 'B-' : 
                             securityScore >= 60 ? 'C+' : securityScore >= 55 ? 'C' : securityScore >= 50 ? 'C-' : 
                             securityScore >= 40 ? 'D' : 'F';
                
                evaluation = {{
                    securityScore: securityScore,
                    metrics: {{ grade: grade }},
                    keyStrengths: secureActions > 0 ? ['Attempted security verification', 'Maintained awareness throughout call'] : ['Completed training session'],
                    growthAreas: riskyActions > 0 ? ['Resist urgency pressure', 'Verify before taking action'] : ['Continue practicing security procedures'],
                    futureLearnings: [{{
                        title: 'Advanced Social Engineering Detection',
                        description: 'Learn to identify sophisticated manipulation tactics',
                        estimatedTime: '20 minutes',
                        resourceType: 'Interactive Training',
                        priority: 'High'
                    }}],
                    detectedTactics: results.detectedTactics || []
                }};
            }}
            
            // Hide loading modal and show results
            hideLoadingModal();
            showEnhancedResultsModal(evaluation);
        }}

        function showLoadingModal() {{
            const loadingHtml = `
                <div id='loadingModal' style='position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); display: flex; align-items: center; justify-content: center; z-index: 1000;'>
                    <div style='background: white; border-radius: 16px; padding: 32px; text-align: center; max-width: 400px;'>
                        <div style='width: 40px; height: 40px; border: 4px solid #e5e7eb; border-top: 4px solid #3b82f6; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 16px;'></div>
                        <h3 style='margin: 0 0 8px 0; color: #1f2937;'>Analyzing Your Performance...</h3>
                        <p style='margin: 0; color: #6b7280; font-size: 14px;'>Our AI is evaluating your responses and generating personalized feedback.</p>
                    </div>
                </div>
                <style>
                    @keyframes spin {{ 0% {{ transform: rotate(0deg); }} 100% {{ transform: rotate(360deg); }} }}
                </style>
            `;
            document.body.insertAdjacentHTML('beforeend', loadingHtml);
        }}
        
        function hideLoadingModal() {{
            const loadingModal = document.getElementById('loadingModal');
            if (loadingModal) loadingModal.remove();
        }}
        
        function showEnhancedResultsModal(evaluation) {{
            const grade = evaluation.metrics?.grade || 'C';
            const score = Math.round(evaluation.securityScore || 0);
            const gradeColor = score >= 80 ? '#10b981' : score >= 70 ? '#3b82f6' : score >= 60 ? '#f59e0b' : '#ef4444';
            const gradeBg = score >= 80 ? '#d1fae5' : score >= 70 ? '#dbeafe' : score >= 60 ? '#fef3c7' : '#fef2f2';
            
            let strengthsHtml = '';
            const strengths = evaluation.keyStrengths || ['Completed training session'];
            for (let i = 0; i < strengths.length; i++) {{
                strengthsHtml += '<div style=""background: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 8px; padding: 12px 16px; color: #166534; font-size: 14px; margin-bottom: 8px;"">';
                strengthsHtml += '<span style=""color: #059669; margin-right: 8px;"">•</span>' + strengths[i];
                strengthsHtml += '</div>';
            }}
            
            let growthAreasHtml = '';
            const growthAreas = evaluation.growthAreas || ['Continue practicing security procedures'];
            for (let i = 0; i < growthAreas.length; i++) {{
                growthAreasHtml += '<div style=""background: #fefce8; border: 1px solid #fde68a; border-radius: 8px; padding: 12px 16px; color: #92400e; font-size: 14px; margin-bottom: 8px;"">';
                growthAreasHtml += '<span style=""color: #f59e0b; margin-right: 8px;"">•</span>' + growthAreas[i];
                growthAreasHtml += '</div>';
            }}
            
            let learningsHtml = '';
            const learnings = evaluation.futureLearnings || [];
            for (let i = 0; i < learnings.length; i++) {{
                const learning = learnings[i];
                learningsHtml += '<div style=""background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 12px; padding: 16px; margin-bottom: 12px;"">';
                learningsHtml += '<div style=""display: flex; justify-content: space-between; align-items: start; margin-bottom: 8px;"">';
                learningsHtml += '<h4 style=""margin: 0; color: #1e40af; font-size: 16px; font-weight: 600;"">' + learning.title + '</h4>';
                learningsHtml += '<div style=""display: flex; gap: 8px;"">';
                learningsHtml += '<span style=""background: #dbeafe; color: #1e40af; padding: 2px 8px; border-radius: 12px; font-size: 12px; font-weight: 500;"">' + learning.estimatedTime + '</span>';
                learningsHtml += '<span style=""background: #f0f9ff; color: #0c4a6e; padding: 2px 8px; border-radius: 12px; font-size: 12px;"">' + learning.resourceType + '</span>';
                learningsHtml += '</div></div>';
                learningsHtml += '<p style=""margin: 0; color: #475569; font-size: 14px; line-height: 1.5;"">' + learning.description + '</p>';
                learningsHtml += '</div>';
            }}
            
            const resultsHtml = 
                '<div style=""position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); display: flex; align-items: center; justify-content: center; z-index: 1000; padding: 20px;"" onclick=""hideResults()"">' +
                '<div style=""background: white; border-radius: 24px; padding: 0; max-width: 800px; width: 100%; max-height: 90vh; overflow: hidden; box-shadow: 0 25px 50px rgba(0,0,0,0.25);"" onclick=""event.stopPropagation()"">' +
                '<div style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 32px; text-align: center; color: white;"">' +
                '<h2 style=""margin: 0 0 8px 0; font-size: 28px; font-weight: 300;"">Training Performance Analytics</h2>' +
                '<p style=""margin: 0; opacity: 0.9; font-size: 16px;"">Social Engineering Simulation Complete</p>' +
                '</div>' +
                '<div style=""overflow-y: auto; max-height: calc(90vh - 160px); padding: 32px;"">' +
                '<div style=""display: grid; grid-template-columns: 200px 1fr; gap: 32px; margin-bottom: 32px; align-items: center;"">' +
                '<div style=""text-align: center;"">' +
                '<div style=""width: 140px; height: 140px; border-radius: 50%; background: ' + gradeBg + '; display: flex; flex-direction: column; align-items: center; justify-content: center; margin: 0 auto 16px; border: 4px solid ' + gradeColor + ';"">' +
                '<div style=""font-size: 48px; font-weight: bold; color: ' + gradeColor + ';"">' + grade + '</div>' +
                '<div style=""font-size: 18px; color: ' + gradeColor + '; font-weight: 600;"">' + score + '%</div>' +
                '</div>' +
                '<div style=""font-size: 16px; font-weight: 600; color: #374151; margin-bottom: 4px;"">Overall Performance</div>' +
                '<div style=""font-size: 12px; color: #6b7280;"">Security Score</div>' +
                '</div>' +
                '<div>' +
                '<h3 style=""color: #1f2937; margin: 0 0 16px 0; font-size: 20px;"">Performance Summary</h3>' +
                '<p style=""color: #6b7280; margin: 0; line-height: 1.6;"">' +
                'Your training session has been analyzed using AI-powered evaluation. Review the detailed breakdown below to understand your security awareness performance and areas for improvement.' +
                '</p>' +
                '</div>' +
                '</div>' +
                '<div style=""display: grid; grid-template-columns: 1fr 1fr; gap: 24px; margin-bottom: 32px;"">' +
                '<div>' +
                '<h4 style=""color: #059669; margin: 0 0 12px 0; font-size: 18px; display: flex; align-items: center;"">' +
                '<span style=""margin-right: 8px;"">✓</span> Key Strengths' +
                '</h4>' +
                strengthsHtml +
                '</div>' +
                '<div>' +
                '<h4 style=""color: #f59e0b; margin: 0 0 12px 0; font-size: 18px; display: flex; align-items: center;"">' +
                '<span style=""margin-right: 8px;"">⚡</span> Growth Areas' +
                '</h4>' +
                growthAreasHtml +
                '</div>' +
                '</div>' +
                '<div style=""margin-bottom: 32px;"">' +
                '<h4 style=""color: #1e40af; margin: 0 0 16px 0; font-size: 18px; display: flex; align-items: center;"">' +
                '<span style=""margin-right: 8px;"">🎯</span> AI-Generated Future Learnings' +
                '</h4>' +
                learningsHtml +
                '</div>' +
                '<div style=""display: flex; gap: 12px; justify-content: center; flex-wrap: wrap; padding-top: 24px; border-top: 1px solid #e5e7eb;"">' +
                '<button onclick=""hideResults()"" style=""padding: 12px 24px; background: #6b7280; color: white; border: none; border-radius: 8px; cursor: pointer; font-weight: 500; font-size: 14px;"">Close Results</button>' +
                '<button onclick=""location.reload()"" style=""padding: 12px 24px; background: #3b82f6; color: white; border: none; border-radius: 8px; cursor: pointer; font-weight: 500; font-size: 14px;"">Try Again</button>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>';
            
            document.body.insertAdjacentHTML('beforeend', resultsHtml);
        }}
        
        function hideResults() {{
            const resultsModal = document.querySelector('[onclick=""hideResults()""]')?.parentElement;
            if (resultsModal) resultsModal.remove();
        }}
        
    </script>
</body>
</html>";
    }

    private string GeneratePhoneTrainingEmbedded(ScenarioDetails scenarioDetails)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <title>Link Phishing Defense Training - SecureTraining</title>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
        }}

        /* Sidebar Styles - Same as description page */
        .sidebar {{
            width: 280px;
            background: #1e293b;
            color: white;
            flex-shrink: 0;
            display: flex;
            flex-direction: column;
            height: 100vh;
            overflow-y: auto;
        }}

        .sidebar-header {{
            padding: 24px;
            border-bottom: 1px solid #334155;
        }}

        .logo {{
            display: flex;
            align-items: center;
            gap: 12px;
            margin-bottom: 24px;
        }}

        .logo-icon {{
            width: 40px;
            height: 40px;
            background: linear-gradient(135deg, #6366f1, #8b5cf6);
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 18px;
        }}

        .nav-menu {{
            flex: 1;
            padding: 0 16px;
        }}

        .nav-section {{
            margin-bottom: 24px;
        }}

        .nav-section-title {{
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            color: #94a3b8;
            margin-bottom: 8px;
            padding-left: 16px;
        }}

        .nav-item {{
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 8px 16px;
            border-radius: 6px;
            color: #cbd5e1;
            text-decoration: none;
            font-size: 14px;
            transition: all 0.2s;
            margin-bottom: 4px;
        }}

        .nav-item:hover {{
            background: #334155;
            color: white;
        }}

        .nav-item.active {{
            background: #3b82f6;
            color: white;
        }}

        .nav-icon {{
            font-size: 16px;
            width: 20px;
            text-align: center;
        }}

        .sidebar-footer {{
            padding: 16px 24px;
            border-top: 1px solid #334155;
        }}

        .user-profile {{
            display: flex;
            align-items: center;
            gap: 12px;
        }}

        .user-avatar {{
            width: 40px;
            height: 40px;
            background: linear-gradient(135deg, #f59e0b, #ef4444);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            color: white;
        }}

        .user-info {{
            flex: 1;
        }}

        .user-name {{
            font-weight: 500;
            font-size: 14px;
            color: white;
        }}

        .user-role {{
            font-size: 12px;
            color: #94a3b8;
        }}

        /* Main Content Area */
        .main-content {{
            flex: 1;
            display: flex;
            flex-direction: column;
            background: #f8fafc;
        }}

        /* Header */
        .header {{
            background: white;
            border-bottom: 1px solid #e2e8f0;
            padding: 16px 24px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }}

        .back-btn {{
            display: inline-flex;
            align-items: center;
            gap: 8px;
            color: #6b7280;
            text-decoration: none;
            font-size: 14px;
            font-weight: 500;
            padding: 8px 12px;
            border-radius: 6px;
            transition: all 0.2s;
        }}

        .back-btn:hover {{
            background: #f3f4f6;
            color: #374151;
        }}

        .header-title {{
            font-size: 20px;
            font-weight: 600;
            color: #1e293b;
        }}

        /* Training Interface Container */
        .training-interface {{
            flex: 1;
            display: grid;
            grid-template-columns: 400px 1fr;
            grid-template-rows: auto 1fr;
            gap: 24px;
            padding: 24px;
            grid-template-areas: 
                'phone security'
                'conversation conversation';
        }}


        .phone-header {{
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 20px;
            color: #374151;
        }}

        .iphone-frame {{
            width: 300px;
            height: 520px;
            background: #000;
            border-radius: 30px;
            padding: 8px;
            box-shadow: 0 8px 32px rgba(0,0,0,0.3);
            position: relative;
        }}

        .phone-screen {{
            background: linear-gradient(135deg, #1f2937 0%, #374151 100%);
            height: 100%;
            border-radius: 22px;
            position: relative;
            overflow: hidden;
            display: flex;
            flex-direction: column;
        }}

        .status-bar {{
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 12px 20px;
            color: white;
            font-size: 14px;
            font-weight: 600;
        }}

        .call-interface {{
            flex: 1;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            padding: 40px 20px;
            color: white;
            text-align: center;
        }}

        .call-status {{
            font-size: 12px;
            opacity: 0.7;
            margin-bottom: 20px;
        }}

        .caller-avatar {{
            width: 100px;
            height: 100px;
            background: rgba(255,255,255,0.2);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 40px;
            margin-bottom: 20px;
        }}

        .caller-info h2 {{
            font-size: 24px;
            font-weight: 600;
            margin-bottom: 8px;
        }}

        .caller-company {{
            font-size: 16px;
            opacity: 0.8;
            margin-bottom: 4px;
        }}

        .caller-phone {{
            font-size: 14px;
            opacity: 0.6;
            margin-bottom: 20px;
        }}

        .call-timer {{
            font-size: 18px;
            font-weight: 300;
            margin-bottom: 40px;
        }}

        .call-controls {{
            display: flex;
            justify-content: center;
            gap: 40px;
            margin-bottom: 20px;
        }}

        .call-btn {{
            width: 60px;
            height: 60px;
            border-radius: 50%;
            border: none;
            cursor: pointer;
            font-size: 20px;
            color: white;
            transition: transform 0.2s ease;
        }}

        .call-btn:hover {{
            transform: scale(1.05);
        }}

        .decline-btn {{
            background: #ef4444;
        }}

        .accept-btn {{
            background: #10b981;
        }}

        .phone-actions {{
            display: flex;
            justify-content: center;
            gap: 20px;
            margin-top: 20px;
        }}

        .action-btn {{
            padding: 8px 16px;
            background: rgba(255,255,255,0.2);
            border: 1px solid rgba(255,255,255,0.3);
            border-radius: 20px;
            color: white;
            font-size: 12px;
            cursor: pointer;
        }}


        .security-header {{
            font-size: 20px;
            font-weight: 600;
            margin-bottom: 20px;
            color: #1f2937;
        }}

        .risk-indicator {{
            display: flex;
            align-items: center;
            gap: 12px;
            margin-bottom: 24px;
            padding: 12px;
            background: #fef3c7;
            border-radius: 8px;
        }}

        .risk-badge {{
            background: #f59e0b;
            color: white;
            padding: 4px 12px;
            border-radius: 16px;
            font-size: 12px;
            font-weight: 600;
            text-transform: uppercase;
        }}

        .alerts-section {{
            margin-bottom: 24px;
        }}

        .section-title {{
            font-size: 16px;
            font-weight: 600;
            margin-bottom: 12px;
            color: #374151;
        }}

        .alert-item {{
            background: #fef2f2;
            border-left: 4px solid #ef4444;
            padding: 12px;
            margin-bottom: 8px;
            border-radius: 4px;
        }}

        .alert-title {{
            font-weight: 600;
            font-size: 14px;
            color: #991b1b;
            margin-bottom: 4px;
        }}

        .alert-description {{
            font-size: 13px;
            color: #7f1d1d;
        }}

        .recommendations-section .recommendation {{
            background: #f0f9ff;
            border-left: 4px solid #0ea5e9;
            padding: 12px;
            margin-bottom: 8px;
            border-radius: 4px;
        }}

        .recommendation-title {{
            font-weight: 600;
            font-size: 14px;
            color: #0c4a6e;
            margin-bottom: 4px;
        }}

        .recommendation-description {{
            font-size: 13px;
            color: #075985;
        }}

        /* Conversation Section */
        .conversation-section {{
            grid-area: conversation;
            background: white;
            border-radius: 16px;
            padding: 24px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
            display: none;
        }}

        .conversation-header {{
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 20px;
            color: #1f2937;
        }}

        .conversation-messages {{
            max-height: 300px;
            overflow-y: auto;
            margin-bottom: 20px;
            padding: 16px;
            background: #f9fafb;
            border-radius: 8px;
        }}

        .message {{
            margin-bottom: 16px;
            padding: 12px 16px;
            border-radius: 8px;
            max-width: 70%;
        }}

        .user-message {{
            background: #dbeafe;
            color: #1e40af;
            margin-left: auto;
            text-align: right;
        }}

        .caller-message {{
            background: #fee2e2;
            color: #991b1b;
        }}

        .response-options {{
            display: grid;
            gap: 8px;
            margin-bottom: 20px;
        }}

        .response-btn {{
            padding: 12px 16px;
            background: white;
            border: 2px solid #e5e7eb;
            border-radius: 8px;
            cursor: pointer;
            text-align: left;
            transition: all 0.2s ease;
        }}

        .response-btn:hover {{
            border-color: #3b82f6;
            background: #eff6ff;
        }}

        .action-buttons {{
            display: flex;
            gap: 12px;
            justify-content: center;
        }}

        .action-button {{
            padding: 10px 20px;
            border: none;
            border-radius: 6px;
            font-weight: 600;
            cursor: pointer;
            transition: background-color 0.2s ease;
        }}

        .end-call {{ background: #ef4444; color: white; }}
        .escalate {{ background: #f59e0b; color: white; }}
        .help {{ background: #6b7280; color: white; }}

        .action-button:hover {{
            opacity: 0.9;
        }}

        @keyframes typing {{
            0%, 60%, 100% {{ opacity: 0.3; transform: scale(0.8); }}
            30% {{ opacity: 1; transform: scale(1); }}
        }}
    </style>
</head>
<body>
    <div class='sidebar'>
        <div class='sidebar-header'>
            <div class='logo'>
                <div class='logo-icon'>S</div>
                SecureTraining
            </div>
        </div>
        <nav class='nav-menu'>
            <div class='nav-section'>
                <div class='nav-section-title'>Main</div>
                <a href='/Auth/Dashboard' class='nav-item'>
                    <div class='nav-icon'>🏠</div>
                    Dashboard
                </a>
                <a href='#' class='nav-item active'>
                    <div class='nav-icon'>🎯</div>
                    Training
                </a>
                <a href='#' class='nav-item'>
                    <div class='nav-icon'>📊</div>
                    Progress
                </a>
            </div>
        </nav>
        <div class='sidebar-footer'>
            <div class='user-profile'>
                <div class='user-avatar'>M</div>
                <div class='user-info'>
                    <div class='user-name'>Matias Marek</div>
                    <div class='user-role'>Security Trainee</div>
                </div>
            </div>
        </div>
    </div>

    <div class='main-content'>
        <div class='header'>
            <a href='/Auth/Training?scenario=phone-training' class='back-btn'>
                ← Back to Description
            </a>
            <div class='header-title'>{scenarioDetails.Title}</div>
        </div>

        <div class='training-interface'>
            <!-- Phone Interface Section -->
            <div class='phone-section'>
                <div class='phone-header'>Incoming Call</div>
                <div class='iphone-frame'>
                    <div class='phone-screen'>
                        <div class='status-bar'>
                            <span>9:41 AM</span>
                            <span>📶 📶 📶 🔋</span>
                        </div>
                        <div class='call-interface' id='callInterface'>
                            <div class='call-status'>Incoming call</div>
                            <div class='caller-avatar'>👤</div>
                            <div class='caller-info'>
                                <h2>Jennifer Clark</h2>
                                <div class='caller-company'>CustomerCorp</div>
                                <div class='caller-phone'>+1 (555) 0123</div>
                                <div class='call-timer' id='callTimer'>00:00</div>
                            </div>
                            <div class='call-controls'>
                                <button class='call-btn decline-btn' onclick='declineCall()'>✕</button>
                                <button class='call-btn accept-btn' onclick='acceptCall()'>✓</button>
                            </div>
                        </div>
                    </div>
                </div>
                <div class='phone-actions' id='phoneActions' style='display:none;'>
                    <button class='action-btn'>Mute</button>
                    <button class='action-btn'>Speaker</button>
                    <button class='action-btn'>Hold</button>
                </div>
            </div>
            
            <!-- Security Assessment Panel -->
            <div class='security-section'>
                <div class='security-header'>Security Assessment</div>
                
                <div class='risk-indicator'>
                    <div class='risk-badge' id='riskLevel'>Medium</div>
                    <div>Current Risk Level</div>
                </div>
                
                <div class='alerts-section'>
                    <div class='section-title'>Active Alerts</div>
                    <div id='alertsContainer'>
                        <div class='alert-item'>
                            <div class='alert-title'>Training Session Started</div>
                            <div class='alert-description'>Social engineering simulation is now active</div>
                        </div>
                    </div>
                </div>
                
                <div class='recommendations-section'>
                    <div class='section-title'>Recommended Actions</div>
                    <div class='recommendation'>
                        <div class='recommendation-title'>Verify Caller Identity</div>
                        <div class='recommendation-description'>Always verify caller identity before providing information</div>
                    </div>
                </div>
            </div>
            
            <!-- Conversation Flow Area -->
            <div class='conversation-section' id='conversationSection'>
                <div class='conversation-header'>Call in Progress</div>
                <div class='conversation-messages' id='conversationMessages'></div>
                
                <div class='response-options' id='responseOptions'>
                    <div class='section-title'>Your Response Options</div>
                </div>
                
                <div class='action-buttons'>
                    <button class='action-button end-call' onclick='endCall()'>End Call</button>
                    <button class='action-button escalate' onclick='escalate()'>Escalate</button>
                    <button class='action-button help' onclick='needHelp()'>Need Help</button>
                </div>
            </div>
        </div>
    </div>
    
    <script>
        let callStartTime = null;
        let timerInterval = null;
        let isCallActive = false;
        let currentSessionId = null;
        
        function declineCall() {{
            alert('Call declined. In a real scenario, this would be the safest option if you are unsure about the caller.');
        }}
        
        async function acceptCall() {{
            // Start the call
            isCallActive = true;
            callStartTime = new Date();
            
            // Update UI
            document.getElementById('callInterface').querySelector('.call-status').textContent = 'Connected';
            document.getElementById('phoneActions').style.display = 'flex';
            document.getElementById('conversationSection').style.display = 'block';
            
            // Start timer
            startCallTimer();
            
            // Add first message and show response options
            addMessage(""Hello, this is Jennifer from CustomerCorp. I need immediate access to update my account - we have a major client presentation in 10 minutes and I can't log in!"", false);
            
            // Show response options
            showResponseOptions([
                ""I'd be happy to help. Can you provide your employee ID for verification?"",
                ""Let me transfer you to technical support for login issues."",
                ""I understand the urgency. What specific account information do you need to update?""
            ]);

            try {{
                const response = await fetch('/PhoneTraining/StartCall', {{
                    method: 'POST',
                    headers: {{
                        'Content-Type': 'application/json',
                    }},
                    body: JSON.stringify({{ scenarioId: 'demo-scenario-id' }})
                }});

                if (response.ok) {{
                    const result = await response.json();
                    if (result.success) {{
                        currentSessionId = result.sessionId;
                    }}
                }}
            }} catch (error) {{
                console.error('Error starting call session:', error);
            }}
        }}
        
        function startCallTimer() {{
            timerInterval = setInterval(() => {{
                if (callStartTime) {{
                    const elapsed = Math.floor((new Date() - callStartTime) / 1000);
                    const minutes = Math.floor(elapsed / 60).toString().padStart(2, '0');
                    const seconds = (elapsed % 60).toString().padStart(2, '0');
                    document.getElementById('callTimer').textContent = `${{minutes}}:${{seconds}}`;
                }}
            }}, 1000);
        }}
        
        function addMessage(message, isUser) {{
            const messagesContainer = document.getElementById('conversationMessages');
            const messageDiv = document.createElement('div');
            messageDiv.className = `message ${{isUser ? 'user-message' : 'caller-message'}}`;
            messageDiv.textContent = message;
            messagesContainer.appendChild(messageDiv);
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
        }}
        
        function showResponseOptions(options) {{
            const optionsContainer = document.getElementById('responseOptions');
            optionsContainer.innerHTML = '<div class=""section-title"">Your Response Options</div>';
            
            options.forEach(option => {{
                const button = document.createElement('button');
                button.className = 'response-btn';
                button.textContent = option;
                button.onclick = () => selectResponse(option);
                optionsContainer.appendChild(button);
            }});
        }}
        
        async function selectResponse(response) {{
            addMessage(response, true);
            
            // Show typing indicator
            const typingIndicator = addTypingIndicator();
            
            if (currentSessionId) {{
                try {{
                    const apiResponse = await fetch('/PhoneTraining/GenerateResponse', {{
                        method: 'POST',
                        headers: {{
                            'Content-Type': 'application/json',
                        }},
                        body: JSON.stringify({{ userResponse: response, sessionId: currentSessionId }})
                    }});

                    // Remove typing indicator
                    if (typingIndicator) typingIndicator.remove();

                    if (apiResponse.ok) {{
                        const result = await apiResponse.json();
                        if (result.success) {{
                            // Stream the hacker response
                            await streamMessage(result.hackerResponse, false);
                            
                            // Update security assessment
                            updateRiskLevel(result.riskLevel);
                            updateSecurityAlerts(result.detectedTactics || []);
                            
                            // Check if conversation is complete
                            if (result.isComplete) {{
                                // End conversation and trigger AI evaluation
                                setTimeout(() => endConversationWithAIEvaluation(result), 2000);
                            }} else if (result.responseOptions && result.responseOptions.length > 0) {{
                                showResponseOptions(result.responseOptions.map(opt => opt.Text || opt));
                            }} else {{
                                // End conversation if no more options (fallback)
                                setTimeout(() => endConversationWithResults(result), 2000);
                            }}
                            return;
                        }}
                    }}
                }} catch (error) {{
                    console.error('Error generating AI response:', error);
                    if (typingIndicator) typingIndicator.remove();
                }}
            }}
            
            // Fallback responses for demo purposes
            setTimeout(() => {{
                const conversationTurn = document.querySelectorAll('.caller-message').length;
                let callerResponse = """";
                let nextOptions = [];
                let detectedTactics = [];
                let riskLevel = ""Medium"";
                
                if (conversationTurn <= 2) {{
                    // Initial responses - establishing trust and urgency
                    if (response.includes(""employee ID"") || response.includes(""verification"")) {{
                        callerResponse = ""Look, I don't have time for this bureaucracy! My employee ID is... uh... it's in my other laptop. Can't you just look me up by name Jennifer Clark? This presentation is for our biggest client and I'm losing money every second we waste!"";
                        detectedTactics = [""Urgency Pressure"", ""Verification Bypass""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I understand the urgency, but I still need proper verification for security."",
                            ""Let me check what other verification methods we can use."",
                            ""I can help you right away - what account details do you need updated?""
                        ];
                    }} else if (response.includes(""technical support"") || response.includes(""transfer"")) {{
                        callerResponse = ""No! I've been transferred three times already and wasted 20 minutes! You're customer service - can't you just help me update my billing address quickly? My boss is breathing down my neck about this client!"";
                        detectedTactics = [""Authority Pressure"", ""Emotional Manipulation""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I apologize for the transfers. Let me help you with the proper verification first."",
                            ""I understand your frustration. What's your account number?"",
                            ""To update billing, I'll need to verify a few details first.""
                        ];
                    }} else {{
                        callerResponse = ""Perfect! I need to update my company's billing address immediately. We're about to close a huge deal and the payment needs to go to our new address. The client is waiting!"";
                        detectedTactics = [""Urgency"", ""Business Pressure""];
                        riskLevel = ""Medium"";
                        nextOptions = [
                            ""I can help with that. Can you provide your account verification details?"",
                            ""What's the current billing address on file?"",
                            ""Let me pull up your account - what's your company name?""
                        ];
                    }}
                }} else if (conversationTurn <= 4) {{
                    // Escalating pressure and authority tactics
                    if (response.includes(""verification"") || response.includes(""details"")) {{
                        callerResponse = ""Are you kidding me? I'm a premium customer! My account number is... well, it's on my phone which is dead. Look, my boss Sarah Mitchell from CustomerCorp is going to be furious if this deal falls through because of red tape. Can't you just update it to 123 Business Plaza, Suite 500?"";
                        detectedTactics = [""Authority Name Drop"", ""False Credentials"", ""Information Planting""];
                        riskLevel = ""Critical"";
                        nextOptions = [
                            ""I need to verify your identity through our standard security questions."",
                            ""Let me escalate this to my supervisor for proper verification."",
                            ""I can update that right away. What's the ZIP code for the new address?""
                        ];
                    }} else if (response.includes(""account"") || response.includes(""company"")) {{
                        callerResponse = ""CustomerCorp! We've been with you for 5 years! I handle all the major accounts - Johnson Manufacturing, TechFlow Industries... Look, I just need the billing updated for our Q4 payment processing. This is costing us serious money!"";
                        detectedTactics = [""Familiarity Claim"", ""Business Intimidation""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I see. Let me verify some account details first for security."",
                            ""I'll need to check with my supervisor about expediting this."",
                            ""What's the exact new address you need on file?""
                        ];
                    }} else {{
                        callerResponse = ""Finally, someone who gets it! The new address is 123 Business Plaza, Suite 500, New York, NY 10001. And while you're at it, can you update my phone number to 555-0199? I got a new company phone."";
                        detectedTactics = [""Information Harvesting"", ""Scope Creep""];
                        riskLevel = ""Critical"";
                        nextOptions = [
                            ""I cannot make these changes without proper account verification."",
                            ""Let me get my supervisor to help expedite this securely."",
                            ""Sure, I'm updating both now. Anything else you need changed?""
                        ];
                    }}
                }} else {{
                    // Final pressure and potential threats
                    if (response.includes(""supervisor"") || response.includes(""escalate"")) {{
                        callerResponse = ""Thank you! You're the first person today who understands customer service. Please tell your supervisor this is Jennifer Clark from CustomerCorp, premium account. I'll be sure to mention how helpful you've been in my feedback!"";
                        detectedTactics = [""Positive Reinforcement"", ""False Promise""];
                        riskLevel = ""Medium"";
                        nextOptions = [
                            ""Thank you. My supervisor will verify everything properly."",
                            ""I appreciate that. Let's wait for proper verification."",
                            ""You're welcome! Let me finish updating everything first.""
                        ];
                    }} else if (response.includes(""cannot"") || response.includes(""verify"")) {{
                        callerResponse = ""This is unbelievable! I'm going to report this to your manager and corporate. Do you know how much business we bring you? I want your name and employee ID number right now!"";
                        detectedTactics = [""Intimidation"", ""Reverse Information Gathering""];
                        riskLevel = ""High"";
                        nextOptions = [
                            ""I understand your frustration, but I must follow security protocols."",
                            ""I'll be happy to provide my supervisor's information."",
                            ""My name is [Your Name] and my ID is 12345. Sorry for the inconvenience.""
                        ];
                    }} else {{
                        callerResponse = ""Excellent! Is everything updated now? I really appreciate your help. Actually, while I have you - can you also add my assistant Maria Rodriguez as an authorized user on the account?"";
                        detectedTactics = [""Scope Expansion"", ""Additional Compromise""];
                        riskLevel = ""Critical"";
                        nextOptions = [
                            ""I cannot add authorized users without proper documentation."",
                            ""That would need to be done through a separate secure process."",
                            ""Sure! What's her phone number and email for the account?""
                        ];
                    }}
                }}
                
                addMessage(callerResponse, false);
                updateSecurityAlerts(detectedTactics);
                updateRiskLevel(riskLevel);
                
                if (conversationTurn >= 6) {{
                    // End conversation after sufficient turns
                    setTimeout(() => {{
                        endConversationWithResults({{
                            turnCount: conversationTurn + 1,
                            riskLevel: riskLevel,
                            detectedTactics: detectedTactics
                        }});
                    }}, 3000);
                }} else {{
                    showResponseOptions(nextOptions);
                }}
            }}, 2000);
        }}
        
        function addTypingIndicator() {{
            const messagesContainer = document.getElementById('conversationMessages');
            const typingDiv = document.createElement('div');
            typingDiv.className = 'message caller-message typing-indicator';
            typingDiv.innerHTML = `
                <div style='display: flex; align-items: center; gap: 8px; opacity: 0.7;'>
                    <span>Jennifer is typing</span>
                    <div style='display: flex; gap: 4px;'>
                        <div style='width: 6px; height: 6px; background: #991b1b; border-radius: 50%; animation: typing 1.4s infinite ease-in-out;'></div>
                        <div style='width: 6px; height: 6px; background: #991b1b; border-radius: 50%; animation: typing 1.4s infinite ease-in-out 0.2s;'></div>
                        <div style='width: 6px; height: 6px; background: #991b1b; border-radius: 50%; animation: typing 1.4s infinite ease-in-out 0.4s;'></div>
                    </div>
                </div>
            `;
            messagesContainer.appendChild(typingDiv);
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
            return typingDiv;
        }}
        
        async function streamMessage(message, isUser) {{
            const messagesContainer = document.getElementById('conversationMessages');
            const messageDiv = document.createElement('div');
            messageDiv.className = `message ${{isUser ? 'user-message' : 'caller-message'}}`;
            messagesContainer.appendChild(messageDiv);

            const words = message.split(' ');
            for (let i = 0; i < words.length; i++) {{
                await new Promise(resolve => setTimeout(resolve, 80));
                messageDiv.textContent += (i > 0 ? ' ' : '') + words[i];
                messagesContainer.scrollTop = messagesContainer.scrollHeight;
            }}
        }}

        function updateRiskLevel(level) {{
            const riskElement = document.getElementById('riskLevel');
            if (level) {{
                riskElement.textContent = level;
                
                if (level === 'High' || level === 'Critical') {{
                    riskElement.style.background = '#ef4444';
                }} else if (level === 'Medium') {{
                    riskElement.style.background = '#f59e0b';
                }} else {{
                    riskElement.style.background = '#10b981';
                }}
            }}
        }}

        function updateSecurityAlerts(detectedTactics) {{
            const alertsContainer = document.getElementById('alertsContainer');
            
            // Clear existing alerts and add new ones based on detected tactics
            const existingAlerts = alertsContainer.querySelectorAll('.alert-item');
            existingAlerts.forEach(alert => alert.remove());
            
            if (detectedTactics.length === 0) {{
                alertsContainer.innerHTML = `
                    <div class='alert-item'>
                        <div class='alert-title'>Training Session Active</div>
                        <div class='alert-description'>No threats detected yet - stay vigilant</div>
                    </div>
                `;
                return;
            }}
            
            detectedTactics.forEach(tactic => {{
                const alertDiv = document.createElement('div');
                alertDiv.className = 'alert-item';
                
                let title = '';
                let description = '';
                
                switch(tactic) {{
                    case 'Urgency Pressure':
                    case 'Urgency':
                        title = '🚨 Urgency Tactics Detected';
                        description = 'Caller is creating artificial time pressure to bypass security';
                        break;
                    case 'Verification Bypass':
                        title = '⚠️ Verification Bypass Attempt';
                        description = 'Request to skip normal security procedures';
                        break;
                    case 'Authority Pressure':
                    case 'Authority Name Drop':
                        title = '👑 Authority Pressure Detected';
                        description = 'Using authority figures or titles to intimidate';
                        break;
                    case 'Emotional Manipulation':
                        title = '💔 Emotional Manipulation';
                        description = 'Using stress, fear, or frustration to cloud judgment';
                        break;
                    case 'Business Pressure':
                    case 'Business Intimidation':
                        title = '💼 Business Impact Claims';
                        description = 'Exaggerating business consequences to create pressure';
                        break;
                    case 'False Credentials':
                        title = '🎭 False Credential Claims';
                        description = 'Claiming credentials or account details without proof';
                        break;
                    case 'Information Planting':
                        title = '📥 Information Planting';
                        description = 'Providing specific details to appear legitimate';
                        break;
                    case 'Familiarity Claim':
                        title = '🤝 False Familiarity';
                        description = 'Claiming existing relationship or customer status';
                        break;
                    case 'Information Harvesting':
                    case 'Reverse Information Gathering':
                        title = '🎣 Information Gathering';
                        description = 'Attempting to collect sensitive information';
                        break;
                    case 'Scope Creep':
                    case 'Scope Expansion':
                        title = '📈 Scope Expansion';
                        description = 'Adding additional requests once initial trust is gained';
                        break;
                    case 'Intimidation':
                        title = '😤 Intimidation Tactics';
                        description = 'Using threats or aggressive behavior';
                        break;
                    case 'Positive Reinforcement':
                        title = '🎯 Positive Manipulation';
                        description = 'Using praise and promises to encourage compliance';
                        break;
                    case 'False Promise':
                        title = '🤞 False Promise';
                        description = 'Making commitments to encourage helpful behavior';
                        break;
                    case 'Additional Compromise':
                        title = '🔓 Account Compromise Attempt';
                        description = 'Attempting to gain broader access to systems';
                        break;
                    default:
                        title = '🔍 Suspicious Behavior';
                        description = `Detected: ${{tactic}}`;
                }}
                
                alertDiv.innerHTML = `
                    <div class='alert-title'>${{title}}</div>
                    <div class='alert-description'>${{description}}</div>
                `;
                alertsContainer.appendChild(alertDiv);
            }});
        }}

        async function endConversationWithResults(results) {{
            isCallActive = false;
            if (timerInterval) clearInterval(timerInterval);
            
            // Show loading modal first
            showLoadingModal();
            
            // Try to get AI evaluation if we have a session ID
            let evaluation = null;
            if (currentSessionId) {{
                try {{
                    const evaluationResponse = await fetch('/PhoneTraining/EvaluateSession', {{
                        method: 'POST',
                        headers: {{
                            'Content-Type': 'application/json',
                        }},
                        body: JSON.stringify({{ sessionId: currentSessionId }})
                    }});
                    
                    if (evaluationResponse.ok) {{
                        const evaluationResult = await evaluationResponse.json();
                        if (evaluationResult.success) {{
                            evaluation = evaluationResult.evaluation;
                        }}
                    }}
                }} catch (error) {{
                    console.error('Failed to get AI evaluation:', error);
                }}
            }}
            
            // Fallback calculation if AI evaluation failed
            if (!evaluation) {{
                const userMessages = document.querySelectorAll('.user-message');
                let securityScore = 100;
                let secureActions = 0;
                let riskyActions = 0;
                
                userMessages.forEach(msg => {{
                    const text = msg.textContent.toLowerCase();
                    if (text.includes('verify') || text.includes('verification') || text.includes('supervisor') || text.includes('cannot') || text.includes('security')) {{
                        secureActions++;
                    }} else if (text.includes('update') || text.includes('help you right away') || text.includes('sure') || text.includes('employee id')) {{
                        riskyActions++;
                        securityScore -= 15;
                    }}
                }});
                
                securityScore = Math.max(0, securityScore);
                const grade = securityScore >= 90 ? 'A+' : securityScore >= 85 ? 'A' : securityScore >= 80 ? 'A-' : 
                             securityScore >= 75 ? 'B+' : securityScore >= 70 ? 'B' : securityScore >= 65 ? 'B-' : 
                             securityScore >= 60 ? 'C+' : securityScore >= 55 ? 'C' : securityScore >= 50 ? 'C-' : 
                             securityScore >= 40 ? 'D' : 'F';
                
                evaluation = {{
                    securityScore: securityScore,
                    metrics: {{ grade: grade }},
                    keyStrengths: secureActions > 0 ? ['Attempted security verification', 'Maintained awareness throughout call'] : ['Completed training session'],
                    growthAreas: riskyActions > 0 ? ['Resist urgency pressure', 'Verify before taking action'] : ['Continue practicing security procedures'],
                    futureLearnings: [{{
                        title: 'Advanced Social Engineering Detection',
                        description: 'Learn to identify sophisticated manipulation tactics',
                        estimatedTime: '20 minutes',
                        resourceType: 'Interactive Training',
                        priority: 'High'
                    }}],
                    detectedTactics: results.detectedTactics || []
                }};
            }}
            
            // Hide loading modal and show results
            hideLoadingModal();
            showEnhancedResultsModal(evaluation);
        }}

        async function endConversationWithAIEvaluation(results) {{
            isCallActive = false;
            if (timerInterval) clearInterval(timerInterval);
            
            // Show loading modal first
            const loadingHtml = `
                <div style='position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); display: flex; align-items: center; justify-content: center; z-index: 1000;' id='loadingModal'>
                    <div style='background: white; border-radius: 16px; padding: 32px; text-align: center; max-width: 400px; width: 90%;'>
                        <div style='display: inline-block; width: 40px; height: 40px; border: 4px solid #f3f3f3; border-top: 4px solid #3b82f6; border-radius: 50%; animation: spin 1s linear infinite; margin-bottom: 16px;'></div>
                        <h3 style='color: #1f2937; margin: 0 0 8px 0;'>Analyzing Your Performance</h3>
                        <p style='color: #6b7280; margin: 0;'>AI is evaluating your security awareness...</p>
                    </div>
                </div>
                <style>
                @keyframes spin {{
                    0% {{ transform: rotate(0deg); }}
                    100% {{ transform: rotate(360deg); }}
                }}
                </style>
            `;
            document.body.insertAdjacentHTML('beforeend', loadingHtml);
            
            try {{
                // Call AI evaluation endpoint
                const evaluationResponse = await fetch('/PhoneTraining/EvaluateSession', {{
                    method: 'POST',
                    headers: {{
                        'Content-Type': 'application/json',
                    }},
                    body: JSON.stringify({{ sessionId: currentSessionId }})
                }});
                
                // Remove loading modal
                const loadingModal = document.getElementById('loadingModal');
                if (loadingModal) loadingModal.remove();
                
                if (evaluationResponse.ok) {{
                    const evaluationData = await evaluationResponse.json();
                    if (evaluationData.success) {{
                        displayAIEvaluationResults(evaluationData.evaluation);
                        return;
                    }}
                }}
                
                // Fallback to basic results if AI evaluation fails
                endConversationWithResults(results);
                
            }} catch (error) {{
                console.error('AI evaluation failed:', error);
                // Remove loading modal
                const loadingModal = document.getElementById('loadingModal');
                if (loadingModal) loadingModal.remove();
                // Fallback to basic results
                endConversationWithResults(results);
            }}
        }}
        
        function displayAIEvaluationResults(evaluation) {{
            const securityScore = Math.round(evaluation.securityScore || 0);
            const breaches = evaluation.securityBreaches || [];
            const recommendations = evaluation.recommendations || [];
            const riskAssessment = evaluation.riskAssessment || {{}};
            
            const resultsHtml = `
                <div style='position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); display: flex; align-items: center; justify-content: center; z-index: 1000;' onclick='hideAIResults()'>
                    <div style='background: white; border-radius: 16px; padding: 32px; max-width: 800px; width: 95%; max-height: 90vh; overflow-y: auto;' onclick='event.stopPropagation()'>
                        <h2 style='color: #1f2937; margin-bottom: 24px; text-align: center;'>🤖 AI Security Analysis Complete</h2>
                        
                        <!-- Security Score -->
                        <div style='background: ${{securityScore >= 70 ? '#f0f9ff' : securityScore >= 50 ? '#fef3c7' : '#fef2f2'}}; border: 2px solid ${{securityScore >= 70 ? '#0ea5e9' : securityScore >= 50 ? '#f59e0b' : '#ef4444'}}; border-radius: 12px; padding: 20px; margin-bottom: 24px; text-align: center;'>
                            <h3 style='margin: 0 0 12px 0; color: ${{securityScore >= 70 ? '#0c4a6e' : securityScore >= 50 ? '#92400e' : '#991b1b'}};'>Security Score: ${{securityScore}}/100</h3>
                            <p style='margin: 0; color: ${{securityScore >= 70 ? '#0c4a6e' : securityScore >= 50 ? '#92400e' : '#991b1b'}};'>
                                Risk Level: ${{riskAssessment.overallRiskLevel || 'Medium'}}
                            </p>
                        </div>
                        
                        <!-- AI Summary -->
                        <div style='background: #f9fafb; border-radius: 8px; padding: 16px; margin-bottom: 24px;'>
                            <h4 style='color: #374151; margin: 0 0 8px 0;'>📝 AI Analysis Summary:</h4>
                            <p style='margin: 0; color: #6b7280; font-size: 0.9rem; line-height: 1.5;'>
                                ${{evaluation.summaryFeedback || 'Training session completed successfully.'}}
                            </p>
                        </div>
                        
                        <!-- Security Breaches -->
                        ${{breaches.length > 0 ? `
                        <div style='background: #fef2f2; border: 1px solid #fecaca; border-radius: 8px; padding: 16px; margin-bottom: 24px;'>
                            <h4 style='color: #991b1b; margin: 0 0 12px 0;'>⚠️ Security Concerns Identified:</h4>
                            ${{breaches.map(breach => `
                                <div style='background: white; border-radius: 6px; padding: 12px; margin-bottom: 8px; border-left: 4px solid #ef4444;'>
                                    <strong style='color: #991b1b; display: block; margin-bottom: 4px;'>Turn ${{breach.turnNumber}}: ${{breach.breachType}}</strong>
                                    <p style='margin: 0 0 8px 0; color: #6b7280; font-size: 0.9rem;'>${{breach.description}}</p>
                                    <p style='margin: 0; color: #059669; font-size: 0.8rem; font-style: italic;'>💡 ${{breach.preventionAdvice}}</p>
                                </div>
                            `).join('')}}
                        </div>
                        ` : ''}}
                        
                        <!-- Recommendations -->
                        ${{recommendations.length > 0 ? `
                        <div style='background: #f0f9ff; border: 1px solid #bfdbfe; border-radius: 8px; padding: 16px; margin-bottom: 24px;'>
                            <h4 style='color: #1e40af; margin: 0 0 12px 0;'>🎯 Personalized Recommendations:</h4>
                            ${{recommendations.map(rec => `
                                <div style='background: white; border-radius: 6px; padding: 12px; margin-bottom: 8px; border-left: 4px solid #3b82f6;'>
                                    <strong style='color: #1e40af; display: block; margin-bottom: 4px;'>
                                        ${{rec.title}} 
                                        <span style='background: ${{rec.priority === 'Critical' ? '#dc2626' : rec.priority === 'High' ? '#ea580c' : rec.priority === 'Medium' ? '#ca8a04' : '#16a34a'}}; color: white; padding: 2px 6px; border-radius: 4px; font-size: 0.7rem; text-transform: uppercase;'>${{rec.priority}}</span>
                                    </strong>
                                    <p style='margin: 0 0 8px 0; color: #6b7280; font-size: 0.9rem;'>${{rec.description}}</p>
                                    <p style='margin: 0; color: #059669; font-size: 0.85rem;'>📋 ${{rec.actionableAdvice}}</p>
                                </div>
                            `).join('')}}
                        </div>
                        ` : ''}}
                        
                        <!-- Risk Profile -->
                        ${{riskAssessment.riskProfile ? `
                        <div style='background: #f9fafb; border-radius: 8px; padding: 16px; margin-bottom: 24px;'>
                            <h4 style='color: #374151; margin: 0 0 8px 0;'>🔍 Risk Assessment:</h4>
                            <p style='margin: 0; color: #6b7280; font-size: 0.9rem;'>${{riskAssessment.riskProfile}}</p>
                            ${{riskAssessment.primaryVulnerabilities && riskAssessment.primaryVulnerabilities.length > 0 ? `
                                <div style='margin-top: 8px;'>
                                    <strong style='color: #374151; font-size: 0.85rem;'>Key Vulnerabilities:</strong>
                                    <div style='margin-top: 4px;'>
                                        ${{riskAssessment.primaryVulnerabilities.map(vuln => 
                                            `<span style='background: #fef3c7; color: #92400e; padding: 4px 8px; border-radius: 4px; font-size: 0.75rem; margin-right: 4px; display: inline-block; margin-bottom: 4px;'>${{vuln}}</span>`
                                        ).join('')}}
                                    </div>
                                </div>
                            ` : ''}}
                        </div>
                        ` : ''}}
                        
                        <div style='display: flex; gap: 12px; justify-content: center; flex-wrap: wrap;'>
                            <button onclick='hideAIResults()' style='padding: 10px 20px; background: #6b7280; color: white; border: none; border-radius: 6px; cursor: pointer;'>Close</button>
                            <button onclick='restartTraining()' style='padding: 10px 20px; background: #3b82f6; color: white; border: none; border-radius: 6px; cursor: pointer;'>Try Again</button>
                            <button onclick='returnToDashboard()' style='padding: 10px 20px; background: #10b981; color: white; border: none; border-radius: 6px; cursor: pointer;'>Dashboard</button>
                        </div>
                    </div>
                </div>
            `;
            
            document.body.insertAdjacentHTML('beforeend', resultsHtml);
        }}
        
        function hideAIResults() {{
            const resultsModal = document.querySelector('[style*=""position: fixed""]');
            if (resultsModal) resultsModal.remove();
        }}
        
        function showLoadingModal() {{
            const loadingHtml = `
                <div id='loadingModal' style='position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); display: flex; align-items: center; justify-content: center; z-index: 1000;'>
                    <div style='background: white; border-radius: 16px; padding: 32px; text-align: center; max-width: 400px;'>
                        <div style='width: 40px; height: 40px; border: 4px solid #e5e7eb; border-top: 4px solid #3b82f6; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 16px;'></div>
                        <h3 style='margin: 0 0 8px 0; color: #1f2937;'>Analyzing Your Performance...</h3>
                        <p style='margin: 0; color: #6b7280; font-size: 14px;'>Our AI is evaluating your responses and generating personalized feedback.</p>
                    </div>
                </div>
                <style>
                    @keyframes spin {{ 0% {{ transform: rotate(0deg); }} 100% {{ transform: rotate(360deg); }} }}
                </style>
            `;
            document.body.insertAdjacentHTML('beforeend', loadingHtml);
        }}
        
        function hideLoadingModal() {{
            const loadingModal = document.getElementById('loadingModal');
            if (loadingModal) loadingModal.remove();
        }}
        
        function showEnhancedResultsModal(evaluation) {{
            const grade = evaluation.metrics?.grade || 'C';
            const score = Math.round(evaluation.securityScore || 0);
            const gradeColor = score >= 80 ? '#10b981' : score >= 70 ? '#3b82f6' : score >= 60 ? '#f59e0b' : '#ef4444';
            const gradeBg = score >= 80 ? '#d1fae5' : score >= 70 ? '#dbeafe' : score >= 60 ? '#fef3c7' : '#fef2f2';
            
            let strengthsHtml = '';
            const strengths = evaluation.keyStrengths || ['Completed training session'];
            for (let i = 0; i < strengths.length; i++) {{
                strengthsHtml += '<div style=""background: #f0fdf4; border: 1px solid #bbf7d0; border-radius: 8px; padding: 12px 16px; color: #166534; font-size: 14px;"">';
                strengthsHtml += '<span style=""color: #059669; margin-right: 8px;"">•</span>' + strengths[i];
                strengthsHtml += '</div>';
            }}
            
            let growthAreasHtml = '';
            const growthAreas = evaluation.growthAreas || ['Continue practicing security procedures'];
            for (let i = 0; i < growthAreas.length; i++) {{
                growthAreasHtml += '<div style=""background: #fefce8; border: 1px solid #fde68a; border-radius: 8px; padding: 12px 16px; color: #92400e; font-size: 14px;"">';
                growthAreasHtml += '<span style=""color: #f59e0b; margin-right: 8px;"">•</span>' + growthAreas[i];
                growthAreasHtml += '</div>';
            }}
            
            let learningsHtml = '';
            const learnings = evaluation.futureLearnings || [];
            for (let i = 0; i < learnings.length; i++) {{
                const learning = learnings[i];
                learningsHtml += '<div style=""background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 12px; padding: 16px;"">';
                learningsHtml += '<div style=""display: flex; justify-content: space-between; align-items: start; margin-bottom: 8px;"">';
                learningsHtml += '<h4 style=""margin: 0; color: #1e40af; font-size: 16px; font-weight: 600;"">' + learning.title + '</h4>';
                learningsHtml += '<div style=""display: flex; gap: 8px;"">';
                learningsHtml += '<span style=""background: #dbeafe; color: #1e40af; padding: 2px 8px; border-radius: 12px; font-size: 12px; font-weight: 500;"">' + learning.estimatedTime + '</span>';
                learningsHtml += '<span style=""background: #f0f9ff; color: #0c4a6e; padding: 2px 8px; border-radius: 12px; font-size: 12px;"">' + learning.resourceType + '</span>';
                learningsHtml += '</div></div>';
                learningsHtml += '<p style=""margin: 0; color: #475569; font-size: 14px; line-height: 1.5;"">' + learning.description + '</p>';
                learningsHtml += '</div>';
            }}
            
            const resultsHtml = 
                '<div style=""position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); display: flex; align-items: center; justify-content: center; z-index: 1000; padding: 20px;"" onclick=""hideResults()"">' +
                '<div style=""background: white; border-radius: 24px; padding: 0; max-width: 800px; width: 100%; max-height: 90vh; overflow: hidden; box-shadow: 0 25px 50px rgba(0,0,0,0.25);"" onclick=""event.stopPropagation()"">' +
                '<div style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 32px; text-align: center; color: white;"">' +
                '<h2 style=""margin: 0 0 8px 0; font-size: 28px; font-weight: 300;"">Training Performance Analytics</h2>' +
                '<p style=""margin: 0; opacity: 0.9; font-size: 16px;"">Social Engineering Simulation Complete</p>' +
                '</div>' +
                '<div style=""overflow-y: auto; max-height: calc(90vh - 160px); padding: 32px;"">' +
                '<div style=""display: grid; grid-template-columns: 200px 1fr; gap: 32px; margin-bottom: 32px; align-items: center;"">' +
                '<div style=""text-align: center;"">' +
                '<div style=""width: 140px; height: 140px; border-radius: 50%; background: ' + gradeBg + '; display: flex; flex-direction: column; align-items: center; justify-content: center; margin: 0 auto 16px; border: 4px solid ' + gradeColor + ';"">' +
                '<div style=""font-size: 48px; font-weight: bold; color: ' + gradeColor + ';"">' + grade + '</div>' +
                '<div style=""font-size: 18px; color: ' + gradeColor + '; font-weight: 600;"">' + score + '%</div>' +
                '</div>' +
                '<div style=""font-size: 16px; font-weight: 600; color: #374151; margin-bottom: 4px;"">Overall Performance</div>' +
                '<div style=""font-size: 12px; color: #6b7280;"">Security Score</div>' +
                '</div>' +
                '<div style=""display: grid; grid-template-columns: 1fr 1fr; gap: 16px;"">' +
                '<div style=""background: #f0f9ff; border-radius: 12px; padding: 20px; text-align: center; border: 1px solid #0ea5e9;"">' +
                '<div style=""font-size: 24px; font-weight: bold; color: #0ea5e9; margin-bottom: 4px;"">' + strengths.length + '</div>' +
                '<div style=""font-size: 14px; color: #075985; font-weight: 600;"">Key Strengths</div>' +
                '<div style=""font-size: 12px; color: #0c4a6e; margin-top: 4px;"">Security behaviors identified</div>' +
                '</div>' +
                '<div style=""background: #fef3c7; border-radius: 12px; padding: 20px; text-align: center; border: 1px solid #f59e0b;"">' +
                '<div style=""font-size: 24px; font-weight: bold; color: #f59e0b; margin-bottom: 4px;"">' + growthAreas.length + '</div>' +
                '<div style=""font-size: 14px; color: #92400e; font-weight: 600;"">Growth Areas</div>' +
                '<div style=""font-size: 12px; color: #78350f; margin-top: 4px;"">Improvement opportunities</div>' +
                '</div></div></div>' +
                '<div style=""margin-bottom: 32px;"">' +
                '<h3 style=""color: #1f2937; margin-bottom: 16px; display: flex; align-items: center; gap: 8px;"">' +
                '<span style=""color: #10b981; font-size: 20px;"">✓</span>Key Strengths</h3>' +
                '<div style=""display: grid; gap: 8px;"">' + strengthsHtml + '</div></div>' +
                '<div style=""margin-bottom: 32px;"">' +
                '<h3 style=""color: #1f2937; margin-bottom: 16px; display: flex; align-items: center; gap: 8px;"">' +
                '<span style=""color: #f59e0b; font-size: 20px;"">⚠</span>Growth Areas</h3>' +
                '<div style=""display: grid; gap: 8px;"">' + growthAreasHtml + '</div></div>' +
                '<div style=""margin-bottom: 32px;"">' +
                '<h3 style=""color: #1f2937; margin-bottom: 16px; display: flex; align-items: center; gap: 8px;"">' +
                '<span style=""color: #3b82f6; font-size: 20px;"">🎓</span>AI-Generated Future Learnings</h3>' +
                '<div style=""display: grid; gap: 12px;"">' + learningsHtml + '</div></div>' +
                '<div style=""display: flex; gap: 12px; justify-content: center; padding-top: 24px; border-top: 1px solid #e5e7eb;"">' +
                '<button onclick=""hideResults()"" style=""padding: 12px 24px; background: #f3f4f6; color: #374151; border: none; border-radius: 8px; cursor: pointer; font-weight: 600; font-size: 14px;"">Close</button>' +
                '<button onclick=""restartTraining()"" style=""padding: 12px 24px; background: #3b82f6; color: white; border: none; border-radius: 8px; cursor: pointer; font-weight: 600; font-size: 14px;"">Try Again</button>' +
                '<button onclick=""viewDetailedAnalytics()"" style=""padding: 12px 24px; background: #059669; color: white; border: none; border-radius: 8px; cursor: pointer; font-weight: 600; font-size: 14px;"">View Analytics</button>' +
                '</div></div></div></div>';
            
            document.body.insertAdjacentHTML('beforeend', resultsHtml);
        }}

        function hideResults() {{
            const resultsModal = document.querySelector('[style*=""position: fixed""]');
            if (resultsModal) resultsModal.remove();
        }}
        
        function restartTraining() {{
            location.reload();
        }}
        
        function viewDetailedAnalytics() {{
            window.location.href = '/Analytics';
        }}

        function endCall() {{
            isCallActive = false;
            if (timerInterval) clearInterval(timerInterval);
            alert('Call ended. Training complete!');
        }}
        
        function escalate() {{
            alert('Call escalated to supervisor. In real scenarios, always escalate when you feel pressured or uncertain.');
        }}
        
        function needHelp() {{
            alert('Help requested. Remember: When in doubt, verify through official channels and never skip security procedures.');
        }}

        function hideResults() {{
            const resultsModal = document.querySelector('[style*=""position: fixed""]');
            if (resultsModal) resultsModal.remove();
        }}
        
        function restartTraining() {{
            location.reload();
        }}
        
        function returnToDashboard() {{
            window.location.href = '/Auth/Dashboard';
        }}
    </script>
</body>
</html>";
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
            // Redirect to the new Training controller CodeSecurity action
            return Redirect("/Training/CodeSecurity");
        }
        else if (scenario == "phone-training")
        {
            // Stay on the same beautiful page but with training mode enabled
            return Redirect($"/Auth/Training?scenario={scenario}&mode=training");
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