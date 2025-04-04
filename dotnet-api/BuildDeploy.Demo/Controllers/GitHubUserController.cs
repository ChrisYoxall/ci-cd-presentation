using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace BuildDeploy.Demo.Controllers;

[ApiController]
[Route("[controller]")]
public class GitHubUserController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public GitHubUserController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    // Visit this in the browser https://api.github.com/users/chrisyoxall
    
    // Would normally create a GitHub service rather than all the code to the controller
    
    [HttpGet("validate/{username}", Name = "ValidateGitHubUser")]
    [ProducesResponseType(typeof(ValidateResponse), 200)] // For successful responses
    [ProducesResponseType(typeof(ApiErrorResponse), 500)] // For server errors
    public async Task<ActionResult<ValidateResponse>> ValidateGitHubUser(string username)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("GitHub");
            var response = await client.GetAsync($"users/{username}");
             
            // If user exists, return validated response with GitHubUser info
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<GitHubUser>();
                return Ok(new ValidateResponse
                {
                    IsValid = true,
                    User = new GitHubUserResponse
                    {
                        Login = user!.Login,
                        Id = user.Id,
                        Name = user.Name,
                        Company = user.Company,
                        PublicRepos = user.PublicRepos
                    }
                });
            }
            
            // If user does not exist, return a validation result with no user data
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return Ok(new ValidateResponse
                {
                    IsValid = false,
                    User = null
                });
            }
            
            // Handle rate limiting or other API errors
            return StatusCode((int)response.StatusCode, new ApiErrorResponse
            {
                Error = $"GitHub API returned status code {response.StatusCode}"
            });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, new ApiErrorResponse
            {
                Error = $"Error connecting to GitHub API: {ex.Message}"
            });
        }
    }
}

/// <summary>
/// Model representing a GitHub user
/// </summary>
public class GitHubUser
{
    [JsonPropertyName("login")]
    public required string Login { get; init; }
        
    [JsonPropertyName("id")]
    public int Id { get; init; }
        
    [JsonPropertyName("name")]
    public string? Name { get; init; }
        
    [JsonPropertyName("company")]
    public string? Company { get; init; }
        
    [JsonPropertyName("public_repos")]
    public int PublicRepos { get; init; }
}

public class GitHubUserResponse
{
    public required string Login { get; init; }
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? Company { get; init; }
    public int PublicRepos { get; init; }
}


/// <summary>
/// Model used for successful responses
/// </summary>
public class ValidateResponse
{
    public bool IsValid { get; init; }
    public required GitHubUserResponse? User { get; init; }
}

/// <summary>
/// Model used for error response
/// </summary>
public class ApiErrorResponse
{
    public required string Error { get;  init; }
}
