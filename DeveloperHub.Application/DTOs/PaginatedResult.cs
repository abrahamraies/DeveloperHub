namespace DeveloperHub.Application.DTOs;
public record PaginatedResult<T>(
	List<T> Items,
	int TotalCount,
	int PageNumber,
	int PageSize);
