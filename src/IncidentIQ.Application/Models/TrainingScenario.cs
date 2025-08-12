using System.ComponentModel.DataAnnotations;

namespace IncidentIQ.Application.Models
{
    public class TrainingScenario
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ScenarioType { get; set; } = "";
        public string TargetRole { get; set; } = "";
        public string Industry { get; set; } = "";
        public string DifficultyLevel { get; set; } = "intermediate";
        public int EstimatedDurationMinutes { get; set; } = 15;
        public List<string> LearningObjectives { get; set; } = new();
        public List<string> ThreatIndicators { get; set; } = new();
        public ScenarioContent Content { get; set; } = new();
        public ScenarioMetrics Metrics { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPersonalized { get; set; } = false;
        public string? CompanyContext { get; set; }
        public List<string> Tags { get; set; } = new();
    }

    public class ScenarioContent
    {
        public string? EmailSubject { get; set; }
        public string? EmailSender { get; set; }
        public string? EmailBody { get; set; }
        public string? PhoneScript { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? AttachmentName { get; set; }
        public List<string> SuspiciousElements { get; set; } = new();
        public List<string> RedFlags { get; set; } = new();
        public string? CorrectAction { get; set; }
        public string? IncorrectConsequence { get; set; }
    }

    public class ScenarioMetrics
    {
        public double SuccessRate { get; set; } = 0.0;
        public double AverageCompletionTime { get; set; } = 0.0;
        public List<string> CommonMistakes { get; set; } = new();
        public double DifficultyRating { get; set; } = 0.5;
        public int TimesUsed { get; set; } = 0;
        public DateTime LastUsed { get; set; } = DateTime.MinValue;
    }

    public class PersonalizedRecommendation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ScenarioType { get; set; } = "";
        public string Priority { get; set; } = "medium"; // high, medium, low
        public string ReasonForRecommendation { get; set; } = "";
        public int EstimatedDurationMinutes { get; set; } = 15;
        public List<string> LearningOutcomes { get; set; } = new();
        public string CompanySpecificContext { get; set; } = "";
        public string RoleSpecificDetails { get; set; } = "";
        public string IndustryContext { get; set; } = "";
        public List<string> PersonalizationFactors { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public bool IsCompleted { get; set; } = false;
        public double? UserScore { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class ScenarioTemplate
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string BaseTitle { get; set; } = "";
        public string BaseDescription { get; set; } = "";
        public string ScenarioType { get; set; } = "";
        public List<string> TargetRoles { get; set; } = new();
        public List<string> ApplicableIndustries { get; set; } = new();
        public string DifficultyLevel { get; set; } = "intermediate";
        public List<PersonalizationVariable> Variables { get; set; } = new();
        public ScenarioContentTemplate ContentTemplate { get; set; } = new();
        public List<string> RequiredContext { get; set; } = new();
    }

    public class PersonalizationVariable
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = ""; // company, role, industry, tool, department
        public string Placeholder { get; set; } = "";
        public bool IsRequired { get; set; } = true;
        public List<string> FallbackValues { get; set; } = new();
    }

    public class ScenarioContentTemplate
    {
        public string? EmailSubjectTemplate { get; set; }
        public string? EmailSenderTemplate { get; set; }
        public string? EmailBodyTemplate { get; set; }
        public string? PhoneScriptTemplate { get; set; }
        public List<string> SuspiciousElementsTemplate { get; set; } = new();
        public List<string> RedFlagsTemplate { get; set; } = new();
        public string? CorrectActionTemplate { get; set; }
        public string? ConsequenceTemplate { get; set; }
    }

    // Specific scenario definitions for different roles
    public static class RoleBasedScenarios
    {
        public static Dictionary<string, List<ScenarioTemplate>> GetScenarioTemplates()
        {
            return new Dictionary<string, List<ScenarioTemplate>>
            {
                ["developer"] = GetDeveloperScenarios(),
                ["marketing"] = GetMarketingScenarios(),
                ["hr"] = GetHRScenarios(),
                ["finance"] = GetFinanceScenarios(),
                ["sales"] = GetSalesScenarios(),
                ["executive"] = GetExecutiveScenarios(),
                ["operations"] = GetOperationsScenarios(),
                ["it"] = GetITScenarios()
            };
        }

        private static List<ScenarioTemplate> GetDeveloperScenarios()
        {
            return new List<ScenarioTemplate>
            {
                new ScenarioTemplate
                {
                    Name = "GitHub Repository Phishing Attack",
                    BaseTitle = "Urgent: Security Vulnerability Found in {COMPANY_NAME} Repository",
                    BaseDescription = "A sophisticated phishing attack targeting {COMPANY_NAME} developers through fake GitHub security notifications, attempting to steal credentials for your {INDUSTRY} company's private repositories.",
                    ScenarioType = "phishing",
                    TargetRoles = new[] { "developer" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Financial Services", "Healthcare" }.ToList(),
                    DifficultyLevel = "challenging",
                    Variables = new List<PersonalizationVariable>
                    {
                        new() { Name = "COMPANY_NAME", Type = "company", Placeholder = "{COMPANY_NAME}", IsRequired = true },
                        new() { Name = "INDUSTRY", Type = "industry", Placeholder = "{INDUSTRY}", IsRequired = true },
                        new() { Name = "REPOSITORY_NAME", Type = "generated", Placeholder = "{REPOSITORY_NAME}", IsRequired = false, FallbackValues = new[] { "core-api", "web-frontend", "mobile-app", "data-pipeline" }.ToList() }
                    },
                    ContentTemplate = new ScenarioContentTemplate
                    {
                        EmailSubjectTemplate = "URGENT: Critical vulnerability detected in {COMPANY_NAME}/{REPOSITORY_NAME}",
                        EmailSenderTemplate = "security-alert@github-security.com",
                        EmailBodyTemplate = @"Dear {COMPANY_NAME} Developer,

Our automated security scanning has detected a critical vulnerability in your {REPOSITORY_NAME} repository that could expose sensitive {INDUSTRY} data.

Immediate Action Required:
→ Click here to review security findings: https://github-security-{COMPANY_NAME}.verify-now.com/urgent
→ Update your credentials to prevent unauthorized access
→ This vulnerability affects 73% of {INDUSTRY} companies

This security alert expires in 2 hours. Please take immediate action to protect {COMPANY_NAME}'s intellectual property.

GitHub Security Team
security@github.com",
                        SuspiciousElementsTemplate = new[] { "Suspicious sender domain", "Urgency tactics", "Credential harvesting link", "Impersonation of trusted service" }.ToList(),
                        RedFlagsTemplate = new[] { "Domain mismatch (github-security.com vs github.com)", "Time pressure tactics", "Asking for credential updates via email", "No repository details provided" }.ToList(),
                        CorrectActionTemplate = "Verify directly through official GitHub notifications, check actual repository security tab, never click suspicious links",
                        ConsequenceTemplate = "Could lead to source code theft, intellectual property loss, and compromise of {COMPANY_NAME}'s development infrastructure"
                    }
                },
                new ScenarioTemplate
                {
                    Name = "Code Review Security Challenge",
                    BaseTitle = "Malicious Code Hidden in {COMPANY_NAME} Pull Request",
                    BaseDescription = "A social engineering attack where malicious code is hidden within a seemingly legitimate pull request to {COMPANY_NAME}'s {INDUSTRY} application, testing your code review security awareness.",
                    ScenarioType = "code_review",
                    TargetRoles = new[] { "developer" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Financial Services", "E-commerce" }.ToList(),
                    DifficultyLevel = "advanced",
                    Variables = new List<PersonalizationVariable>
                    {
                        new() { Name = "COMPANY_NAME", Type = "company", Placeholder = "{COMPANY_NAME}", IsRequired = true },
                        new() { Name = "INDUSTRY", Type = "industry", Placeholder = "{INDUSTRY}", IsRequired = true }
                    }
                },
                new ScenarioTemplate
                {
                    Name = "Production Database Social Engineering",
                    BaseTitle = "Emergency Database Access Request for {COMPANY_NAME}",
                    BaseDescription = "A social engineering attack targeting {COMPANY_NAME} developers with urgent requests for production database access, exploiting {INDUSTRY} compliance pressures and time sensitivity.",
                    ScenarioType = "social_engineering",
                    TargetRoles = new[] { "developer" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Healthcare", "Financial Services" }.ToList(),
                    DifficultyLevel = "challenging"
                }
            };
        }

        private static List<ScenarioTemplate> GetMarketingScenarios()
        {
            return new List<ScenarioTemplate>
            {
                new ScenarioTemplate
                {
                    Name = "Vendor Invoice Fraud",
                    BaseTitle = "Updated Payment Information - {COMPANY_NAME} Marketing Services",
                    BaseDescription = "A sophisticated invoice fraud targeting {COMPANY_NAME}'s marketing team, impersonating legitimate {INDUSTRY} vendors to redirect payments to fraudulent accounts.",
                    ScenarioType = "invoice_fraud",
                    TargetRoles = new[] { "marketing" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Retail", "Manufacturing" }.ToList(),
                    DifficultyLevel = "intermediate"
                },
                new ScenarioTemplate
                {
                    Name = "Social Media Account Takeover",
                    BaseTitle = "Urgent: {COMPANY_NAME} Brand Security Alert",
                    BaseDescription = "A phishing attack designed to compromise {COMPANY_NAME}'s social media accounts by targeting marketing professionals with fake security alerts about {INDUSTRY} brand threats.",
                    ScenarioType = "account_takeover",
                    TargetRoles = new[] { "marketing" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Retail", "Media" }.ToList(),
                    DifficultyLevel = "intermediate"
                },
                new ScenarioTemplate
                {
                    Name = "Customer Data Phishing",
                    BaseTitle = "Customer Complaints About {COMPANY_NAME} Data Breach",
                    BaseDescription = "A social engineering attack exploiting {COMPANY_NAME}'s reputation concerns in the {INDUSTRY} sector, attempting to harvest customer data through fake complaint investigations.",
                    ScenarioType = "data_phishing",
                    TargetRoles = new[] { "marketing" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "E-commerce", "Healthcare" }.ToList(),
                    DifficultyLevel = "challenging"
                }
            };
        }

        private static List<ScenarioTemplate> GetHRScenarios()
        {
            return new List<ScenarioTemplate>
            {
                new ScenarioTemplate
                {
                    Name = "Employee Verification Social Engineering",
                    BaseTitle = "Employment Verification Request - {COMPANY_NAME} Employee",
                    BaseDescription = "A sophisticated social engineering attack targeting {COMPANY_NAME}'s HR team, attempting to extract sensitive employee information through fake employment verification requests in the {INDUSTRY} sector.",
                    ScenarioType = "social_engineering",
                    TargetRoles = new[] { "hr" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Healthcare", "Financial Services" }.ToList(),
                    DifficultyLevel = "intermediate"
                },
                new ScenarioTemplate
                {
                    Name = "Resume Malware Attack",
                    BaseTitle = "Executive Resume - {COMPANY_NAME} Leadership Opportunity",
                    BaseDescription = "A malware attack disguised as an executive resume submission to {COMPANY_NAME}, targeting HR professionals with industry-specific credentials for {INDUSTRY} leadership positions.",
                    ScenarioType = "malware",
                    TargetRoles = new[] { "hr" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Financial Services", "Healthcare" }.ToList(),
                    DifficultyLevel = "challenging"
                },
                new ScenarioTemplate
                {
                    Name = "Payroll System Phishing",
                    BaseTitle = "Urgent: {COMPANY_NAME} Payroll System Security Update",
                    BaseDescription = "A targeted phishing attack against {COMPANY_NAME}'s payroll systems, exploiting HR's administrative access and {INDUSTRY} compliance requirements to steal employee financial data.",
                    ScenarioType = "phishing",
                    TargetRoles = new[] { "hr" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Manufacturing", "Healthcare" }.ToList(),
                    DifficultyLevel = "advanced"
                }
            };
        }

        private static List<ScenarioTemplate> GetFinanceScenarios()
        {
            return new List<ScenarioTemplate>
            {
                new ScenarioTemplate
                {
                    Name = "CEO Wire Transfer Fraud",
                    BaseTitle = "Confidential Acquisition - Immediate Wire Transfer Required",
                    BaseDescription = "A business email compromise attack impersonating {COMPANY_NAME}'s CEO, requesting urgent wire transfers for a fake {INDUSTRY} acquisition deal.",
                    ScenarioType = "wire_fraud",
                    TargetRoles = new[] { "finance" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Manufacturing", "Real Estate" }.ToList(),
                    DifficultyLevel = "advanced"
                },
                new ScenarioTemplate
                {
                    Name = "Vendor Payment Redirect",
                    BaseTitle = "Banking Details Update - {COMPANY_NAME} Vendor Payment",
                    BaseDescription = "A sophisticated payment fraud targeting {COMPANY_NAME}'s finance team, redirecting legitimate vendor payments through fake banking updates in the {INDUSTRY} supply chain.",
                    ScenarioType = "payment_fraud",
                    TargetRoles = new[] { "finance" }.ToList(),
                    ApplicableIndustries = new[] { "Manufacturing", "Technology", "Healthcare" }.ToList(),
                    DifficultyLevel = "challenging"
                }
            };
        }

        private static List<ScenarioTemplate> GetSalesScenarios()
        {
            return new List<ScenarioTemplate>
            {
                new ScenarioTemplate
                {
                    Name = "Client Impersonation Attack",
                    BaseTitle = "Contract Amendment - {COMPANY_NAME} Partnership",
                    BaseDescription = "A social engineering attack impersonating a major {COMPANY_NAME} client in the {INDUSTRY} sector, attempting to manipulate contract terms or steal confidential proposal information.",
                    ScenarioType = "client_impersonation",
                    TargetRoles = new[] { "sales" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Professional Services", "Manufacturing" }.ToList(),
                    DifficultyLevel = "intermediate"
                }
            };
        }

        private static List<ScenarioTemplate> GetExecutiveScenarios()
        {
            return new List<ScenarioTemplate>
            {
                new ScenarioTemplate
                {
                    Name = "Strategic Information Theft",
                    BaseTitle = "Confidential: {COMPANY_NAME} Board Meeting Materials",
                    BaseDescription = "An advanced persistent threat targeting {COMPANY_NAME} executives, attempting to steal strategic information about {INDUSTRY} market expansion and competitive intelligence.",
                    ScenarioType = "strategic_theft",
                    TargetRoles = new[] { "executive" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Financial Services", "Healthcare" }.ToList(),
                    DifficultyLevel = "expert"
                }
            };
        }

        private static List<ScenarioTemplate> GetOperationsScenarios()
        {
            return new List<ScenarioTemplate>
            {
                new ScenarioTemplate
                {
                    Name = "Supply Chain Disruption Attack",
                    BaseTitle = "Supply Chain Alert - {COMPANY_NAME} Vendor Compromise",
                    BaseDescription = "A supply chain attack targeting {COMPANY_NAME}'s operations team, exploiting vendor relationships in the {INDUSTRY} sector to gain unauthorized access to systems.",
                    ScenarioType = "supply_chain",
                    TargetRoles = new[] { "operations" }.ToList(),
                    ApplicableIndustries = new[] { "Manufacturing", "Technology", "Retail" }.ToList(),
                    DifficultyLevel = "challenging"
                }
            };
        }

        private static List<ScenarioTemplate> GetITScenarios()
        {
            return new List<ScenarioTemplate>
            {
                new ScenarioTemplate
                {
                    Name = "Privileged Access Attack",
                    BaseTitle = "System Administrator Alert - {COMPANY_NAME} Infrastructure",
                    BaseDescription = "A sophisticated attack targeting {COMPANY_NAME}'s IT administrators, attempting to compromise privileged access to critical {INDUSTRY} infrastructure and systems.",
                    ScenarioType = "privilege_escalation",
                    TargetRoles = new[] { "it" }.ToList(),
                    ApplicableIndustries = new[] { "Technology", "Financial Services", "Healthcare" }.ToList(),
                    DifficultyLevel = "expert"
                }
            };
        }
    }
}