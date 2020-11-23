using Database;
using Database.Models;
using FakeItEasy;
using LogParser.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LogParserTests.ViewModels
{
	public class LogParserViewModelTests
	{
		[Fact]
		public void LogParserViewModel_ConstructorNull_Test()
		{
			// Arrange
			Action viewModel = () => new LogParserViewModel(null, null);

			// Act & Assert
			Assert.Throws<ArgumentNullException>("dbContext", viewModel);
		}

		[Fact]
		public void LogParserViewModel_Constructor_Test()
		{
			// Arrange
			var fakeContext = A.Fake<DatabaseContext>(
				x => x.WithArgumentsForConstructor(() => new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>().Options)));

			// Act
			LogParserViewModel viewModel = new LogParserViewModel(fakeContext, null);

			// Assert
			Assert.NotNull(viewModel);
			Assert.NotNull(viewModel.LogFiles);
		}
	}
}
