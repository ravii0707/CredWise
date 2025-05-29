public class EnquiryDto
{
    public int EnquiryId { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public string Phone { get; set; }
    public string Message { get; set; }
    public string? Status { get; set; } // "Active", "Inactive", "Resolved"
    public DateTime CreatedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class ToggleEnquiryStatusDto
{
    public int EnquiryId { get; set; }
    public string NewStatus { get; set; } // "Active", "Inactive", "Resolved"
    public string? ResolutionNote { get; set; }
}
