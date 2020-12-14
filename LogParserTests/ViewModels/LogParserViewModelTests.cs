using Database;
using Database.Models;
using FakeItEasy;
using LogParser.Services;
using LogParser.Resources;
using LogParser.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LogParserTests.ViewModels
{
	public class LogParserViewModelTests
	{
		[Fact]
		public void Constructor_DbNull_Test()
		{
			// Arrange
			var dummyParseController = A.Dummy<ParseService>();
			var dummyDpsReportController = A.Dummy<DpsReportController>();

			// Act
			//Action viewModel = () => new LogParserViewModel(null, dummyParseController, dummyDpsReportController);

			//// Assert
			//Assert.Throws<ArgumentNullException>("dbContext", viewModel);
		}

		[Fact]
		public void Constructor_ParseControllerNull_Test()
		{
			// Arrange
			var dummyContext = A.Fake<DatabaseContext>(
				x => x.WithArgumentsForConstructor(() => new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>().Options)));
			var dummyDpsReportController = A.Dummy<DpsReportController>();

			// Act
			//Action viewModel = () => new LogParserViewModel(dummyContext, null, dummyDpsReportController);

			//// Assert
			//Assert.Throws<ArgumentNullException>("parseService", viewModel);
		}

		[Fact]
		public void Constructor_DpsReportControllerNull_Test()
		{
			// Arrange
			var dummyContext = A.Fake<DatabaseContext>(
				x => x.WithArgumentsForConstructor(() => new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>().Options)));
			var dummyParseController = A.Dummy<ParseService>();

			// Act
			//Action viewModel = () => new LogParserViewModel(dummyContext, dummyParseController, null);

			//// Assert
			//Assert.Throws<ArgumentNullException>("dpsReportController", viewModel);
		}

		[Fact]
		public void Constructor_Test()
		{
			// Arrange
			var dummyContext = A.Fake<DatabaseContext>(
				x => x.WithArgumentsForConstructor(() => new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>().Options)));
			var dummyParseController = A.Dummy<ParseService>();
			var dummyDpsReportController = A.Dummy<DpsReportController>();

			// Act
			//var viewModel = new LogParserViewModel(dummyContext, dummyParseController, dummyDpsReportController);

			//// Assert
			//Assert.NotNull(viewModel);
			//Assert.NotNull(viewModel.LogFiles);
			//Assert.NotNull(viewModel.BossNameFilters);
			//Assert.True(viewModel.BossNameFilters.Count == 1);
			//Assert.NotNull(viewModel.FilesToParse);
			//Assert.Equal(Resource.EliteInsightsNotInstalled, viewModel.EliteInsightsVersion);
			//Assert.Equal(Resource.InstallEliteInsights, viewModel.UpdateEliteInsightsContent);
		}

		[Fact]
		public void Constructor_EliteInsightsInstalled_Test()
		{
			// Arrange
			var dummyContext = A.Fake<DatabaseContext>(
				x => x.WithArgumentsForConstructor(() => new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>().Options)));
			var fakeParseController = A.Fake<IParseService>();
			var dummyDpsReportController = A.Dummy<DpsReportController>();

			string fileVersion = "1.2.3.4";
			A.CallTo(() => fakeParseController.GetFileVersionInfo()).Returns(fileVersion);

			// Act
			//var viewModel = new LogParserViewModel(dummyContext, fakeParseController, dummyDpsReportController);

			//// Assert
			//Assert.NotNull(viewModel);
			//Assert.NotNull(viewModel.LogFiles);
			//Assert.NotNull(viewModel.BossNameFilters);
			//Assert.True(viewModel.BossNameFilters.Count == 1);
			//Assert.NotNull(viewModel.FilesToParse);
			//Assert.Equal(fileVersion, viewModel.EliteInsightsVersion);
			//Assert.Equal(Resource.UpdateEliteInsights, viewModel.UpdateEliteInsightsContent);
		}

		[Fact]
		public void ParseFilesAsync_Test()
		{
			// Arrange
			var dummyContext = A.Fake<DatabaseContext>(
				x => x.WithArgumentsForConstructor(() => new DatabaseContext(new DbContextOptionsBuilder<DatabaseContext>().Options)));
			var fakeParseController = A.Fake<IParseService>();
			var dummyDpsReportController = A.Dummy<DpsReportController>();

			//var viewModel = new LogParserViewModel(dummyContext, fakeParseController, dummyDpsReportController);

			//// Act
			//viewModel.ParseFilesAsync().Wait();

			// Assert
		}
	}
}
