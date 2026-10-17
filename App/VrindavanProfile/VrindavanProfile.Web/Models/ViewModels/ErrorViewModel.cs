namespace VrindavanProfile.Web.Models.ViewModels;

/// <summary>
/// Error page view model (Part 13A).
/// Used by Error, NotFound, and Forbidden views.
/// </summary>
public class ErrorViewModel
{
    public int StatusCode { get; set; } = 500;
    public string RequestId { get; set; } = string.Empty;
    public string RequestPath { get; set; } = string.Empty;
    public bool ShowException { get; set; } = false;
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
