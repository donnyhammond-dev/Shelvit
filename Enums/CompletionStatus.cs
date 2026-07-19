using System.ComponentModel.DataAnnotations;

namespace BackLog.Enums
{
    public enum CompletionStatus
    {
        [Display(Name = "Not Started")]
        NotStarted,
        [Display(Name = "Incomplete")]
        Incomplete,
        [Display(Name = "Completed")]
        Completed,
        [Display(Name = "Abandoned")]
        Abandoned
    }
}
