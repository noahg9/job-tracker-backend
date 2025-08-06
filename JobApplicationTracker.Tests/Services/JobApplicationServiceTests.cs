using Xunit;
using Moq;
using FluentAssertions;
using JobApplicationTracker.Api.Services;
using JobApplicationTracker.Api.Repositories;
using JobApplicationTracker.Api.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;

public class JobApplicationServiceTests
{
	private readonly Mock<IJobApplicationRepository> _mockRepo;
	private readonly Mock<ILogger<JobApplicationService>> _mockLogger;
	private readonly JobApplicationService _service;

	public JobApplicationServiceTests()
	{
		_mockRepo = new Mock<IJobApplicationRepository>();
		_mockLogger = new Mock<ILogger<JobApplicationService>>();
		_service = new JobApplicationService(_mockRepo.Object, _mockLogger.Object);
	}

	[Fact]
	public async Task GetAllAsync_ShouldReturnAllJobs()
	{
		// Arrange
		var mockData = new List<JobApplication>
		{
			new JobApplication { Id = 1, Company = "Acme", Role = "Dev", Status = ApplicationStatus.Applied },
			new JobApplication { Id = 2, Company = "Beta", Role = "QA", Status = ApplicationStatus.Interview }
		};
		_mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(mockData);

		// Act
		var result = await _service.GetAllAsync();

		// Assert
		result.Should().HaveCount(2);
		result[0].Company.Should().Be("Acme");
	}

	[Fact]
	public async Task GetByIdAsync_ExistingId_ShouldReturnJob()
	{
		// Arrange
		var job = new JobApplication { Id = 1, Company = "Acme", Role = "Dev", Status = ApplicationStatus.Applied };
		_mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(job);

		// Act
		var result = await _service.GetByIdAsync(1);

		// Assert
		result.Should().NotBeNull();
		result.Company.Should().Be("Acme");
	}

	[Fact]
	public async Task DeleteAsync_NonExistingId_ShouldReturnFalse()
	{
		// Arrange
		_mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((JobApplication?)null);

		// Act
		var result = await _service.DeleteAsync(99);

		// Assert
		result.Should().BeFalse();
	}

	[Fact]
	public async Task UpdateAsync_MismatchedId_ShouldReturnFalse()
	{
		// Arrange
		var job = new JobApplication { Id = 2, Company = "Test" };

		// Act
		var result = await _service.UpdateAsync(1, job); // IDs don't match

		// Assert
		result.Should().BeFalse();
	}
}
