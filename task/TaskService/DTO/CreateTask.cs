using System.ComponentModel.DataAnnotations;

namespace TaskService.DTO;
public record CreateTask
(
    [Required]string Title,
    string Description
);
