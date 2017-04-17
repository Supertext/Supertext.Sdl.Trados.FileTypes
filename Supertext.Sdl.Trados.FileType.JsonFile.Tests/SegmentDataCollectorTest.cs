﻿using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Sdl.FileTypeSupport.Framework.Core.Settings;
using Supertext.Sdl.Trados.FileType.JsonFile.Settings;

namespace Supertext.Sdl.Trados.FileType.JsonFile.Tests
{
    [TestFixture]
    public class SegmentDataCollectorTest
    {
        public static List<TestCase> TestCases = new List<TestCase>
        {
            new TestCase
            {
                Name = "TestCase1",
                PathRules = new ComplexObservableList<PathRule>
                {
                    new PathRule
                    {
                        SourcePathPattern = "TheSourcePath",
                        TargetPathPattern = "TheTargetPath"
                    }
                },
                AddChecks = new List<AddCheck>
                {
                  new AddCheck
                  {
                      Path = "TheSourcePath",
                      Value = "The source value"
                  },
                  new AddCheck
                  {
                      Path = "TheTargetPath",
                      Value = "The target value",
                      ExpectedSegmentData = new SegmentData
                      {
                          SourcePath = "TheSourcePath",
                          SourceValue = "The source value",
                          TargetPath = "TheTargetPath",
                          TargetValue = "The target value"
                      }
                  }
                }
            },
            new TestCase
            {
                Name = "TestCase2",
                PathRules = new ComplexObservableList<PathRule>
                {
                    new PathRule
                    {
                        SourcePathPattern = "TheSourcePath",
                        TargetPathPattern = "TheTargetPath"
                    }
                },
                AddChecks = new List<AddCheck>
                {
                    new AddCheck
                    {
                        Path = "SomeOtherPath",
                        Value = "The others path value"
                    },
                    new AddCheck
                    {
                        Path = "SomeCompletlyOtherPath",
                        Value = "The completly other value"
                    }
                }
            },
            new TestCase
            {
                Name = "TestCase3",
                PathRules = new ComplexObservableList<PathRule>
                {
                    new PathRule
                    {
                        SourcePathPattern = "TheSourcePath1",
                        TargetPathPattern = "TheTargetPath1"
                    },
                    new PathRule
                    {
                        SourcePathPattern = "TheSourcePath2",
                        TargetPathPattern = "TheTargetPath2"
                    }
                },
                AddChecks = new List<AddCheck>
                {
                    new AddCheck
                    {
                        Path = "TheSourcePath1",
                        Value = "The 1. source value"
                    },
                    new AddCheck
                    {
                        Path = "TheTargetPath1",
                        Value = "The 1. target value",
                        ExpectedSegmentData = new SegmentData
                        {
                            SourcePath = "TheSourcePath1",
                            SourceValue = "The 1. source value",
                            TargetPath = "TheTargetPath1",
                            TargetValue = "The 1. target value"
                        }
                    },
                    new AddCheck
                    {
                        Path = "TheSourcePath2",
                        Value = "The 2. source value"
                    },
                    new AddCheck
                    {
                        Path = "TheTargetPath2",
                        Value = "The 2. target value",
                        ExpectedSegmentData = new SegmentData
                        {
                            SourcePath = "TheSourcePath2",
                            SourceValue = "The 2. source value",
                            TargetPath = "TheTargetPath2",
                            TargetValue = "The 2. target value"
                        }
                    }
                }
            },
            new TestCase
            {
                Name = "TestCase4",
                PathRules = new ComplexObservableList<PathRule>
                {
                    new PathRule
                    {
                        SourcePathPattern = "TheSourcePath1",
                        TargetPathPattern = "TheTargetPath1"
                    },
                    new PathRule
                    {
                        SourcePathPattern = "TheSourcePath2",
                        TargetPathPattern = "TheTargetPath2"
                    }
                },
                AddChecks = new List<AddCheck>
                {
                    new AddCheck
                    {
                        Path = "TheSourcePath1",
                        Value = "The 1. source value"
                    },
                    new AddCheck
                    {
                        Path = "TheSourcePath2",
                        Value = "The 2. source value"
                    },
                    new AddCheck
                    {
                        Path = "TheTargetPath1",
                        Value = "The 1. target value",
                        ExpectedSegmentData = new SegmentData
                        {
                            SourcePath = "TheSourcePath1",
                            SourceValue = "The 1. source value",
                            TargetPath = "TheTargetPath1",
                            TargetValue = "The 1. target value"
                        }
                    },
                    new AddCheck
                    {
                        Path = "TheTargetPath2",
                        Value = "The 2. target value",
                        ExpectedSegmentData = new SegmentData
                        {
                            SourcePath = "TheSourcePath2",
                            SourceValue = "The 2. source value",
                            TargetPath = "TheTargetPath2",
                            TargetValue = "The 2. target value"
                        }
                    }
                }
            },
            new TestCase
            {
                Name = "TestCase5",
                PathRules = new ComplexObservableList<PathRule>
                {
                    new PathRule
                    {
                        SourcePathPattern = "TheSourcePath1",
                        TargetPathPattern = "TheTargetPath1"
                    }
                },
                AddChecks = new List<AddCheck>
                {
                    new AddCheck
                    {
                        Path = "TheTargetPath1",
                        Value = "The 1. target value"
                        
                    },
                    new AddCheck
                    {
                        Path = "TheSourcePath1",
                        Value = "The 1. source value",
                        ExpectedSegmentData = new SegmentData
                        {
                            SourcePath = "TheSourcePath1",
                            SourceValue = "The 1. source value",
                            TargetPath = "TheTargetPath1",
                            TargetValue = "The 1. target value"
                        }
                    }
                }
            },
            new TestCase
            {
                Name = "TestCase6",
                PathRules = new ComplexObservableList<PathRule>
                {
                    new PathRule
                    {
                        SourcePathPattern = @"TheBasePath.Array1Property\[\d\].Source",
                        TargetPathPattern = @"TheBasePath.Array1Property\[\d\].Target"
                    },
                    new PathRule
                    {
                        SourcePathPattern = @"TheBasePath.Array2Property\[\d\].Source",
                        TargetPathPattern = @"TheBasePath.Array2Property\[\d\].Target"
                    }
                },
                AddChecks = new List<AddCheck>
                {
                    new AddCheck
                    {
                        Path = "TheBasePath.Array1Property[0].Source",
                        Value = "The source value of first array first item"

                    },
                    new AddCheck
                    {
                        Path = "TheBasePath.Array1Property[1].Source",
                        Value = "The source value of first array second item"

                    },
                    new AddCheck
                    {
                        Path = "TheBasePath.Array1Property[0].Target",
                        Value = "The target value of first array first item",
                        ExpectedSegmentData = new SegmentData
                        {
                            SourcePath = "TheBasePath.Array1Property[0].Source",
                            SourceValue = "The source value of first array first item",
                            TargetPath = "TheBasePath.Array1Property[0].Target",
                            TargetValue = "The target value of first array first item"
                        }
                    },
                    new AddCheck
                    {
                        Path = "TheBasePath.Array1Property[1].Target",
                        Value = "The target value of first array second item",
                        ExpectedSegmentData = new SegmentData
                        {
                            SourcePath = "TheBasePath.Array1Property[1].Source",
                            SourceValue = "The source value of first array second item",
                            TargetPath = "TheBasePath.Array1Property[1].Target",
                            TargetValue = "The target value of first array second item"
                        }
                    }
                }
            },
            new TestCase
            {
                Name = "TestCase7",
                PathRules = new ComplexObservableList<PathRule>
                {
                    new PathRule
                    {
                        SourcePathPattern = @"Source",
                        TargetPathPattern = @"Target"
                    }
                },
                AddChecks = new List<AddCheck>
                {
                    new AddCheck
                    {
                        Path = "TheBasePath.Array1Property[0].Source",
                        Value = "The source value of first array first item"

                    },
                    new AddCheck
                    {
                        Path = "TheBasePath.Array1Property[1].Target",
                        Value = "The target value of first array second item"
                    },
                    new AddCheck
                    {
                        Path = "TheBasePath.Array1Property[1].Source",
                        Value = "The source value of first array second item",
                        ExpectedSegmentData = new SegmentData
                        {
                            SourcePath = "TheBasePath.Array1Property[1].Source",
                            SourceValue = "The source value of first array second item",
                            TargetPath = "TheBasePath.Array1Property[1].Target",
                            TargetValue = "The target value of first array second item"
                        }
                    },
                    new AddCheck
                    {
                        Path = "TheBasePath.Array1Property[0].Target",
                        Value = "The target value of first array first item",
                        ExpectedSegmentData = new SegmentData
                        {
                            SourcePath = "TheBasePath.Array1Property[0].Source",
                            SourceValue = "The source value of first array first item",
                            TargetPath = "TheBasePath.Array1Property[0].Target",
                            TargetValue = "The target value of first array first item"
                        }
                    },
                    
                }
            },
            new TestCase
            {
                Name = "TestCase8",
                PathRules = new ComplexObservableList<PathRule>
                {
                    new PathRule
                    {
                        SourcePathPattern = "TheSourcePath",
                        TargetPathPattern = "TheTargetPath",
                        IgnoreCase = false
                    }
                },
                AddChecks = new List<AddCheck>
                {
                    new AddCheck
                    {
                        Path = "thesourcepath",
                        Value = "The source value"
                    },
                    new AddCheck
                    {
                        Path = "TheTargetPath",
                        Value = "The target value"
                    }
                }
            },
            new TestCase
            {
                Name = "TestCase9",
                PathRules = new ComplexObservableList<PathRule>
                {
                    new PathRule
                    {
                        SourcePathPattern = "TheSourcePath",
                        TargetPathPattern = "TheTargetPath",
                        IgnoreCase = true
                    }
                },
                AddChecks = new List<AddCheck>
                {
                    new AddCheck
                    {
                        Path = "thesourcepath",
                        Value = "The source value"
                    },
                    new AddCheck
                    {
                        Path = "TheTargetPath",
                        Value = "The target value",
                        ExpectedSegmentData = new SegmentData
                        {
                            SourcePath = "thesourcepath",
                            SourceValue = "The source value",
                            TargetPath = "TheTargetPath",
                            TargetValue = "The target value"
                        }
                    }
                }
            },
        };

        [Test, TestCaseSource(nameof(TestCases))]
        public void Add_WhenPathFilteringIsEnabled_ShouldSetCompleteSegmentDataWhenComplete(TestCase testCase)
        {
            // Arrange

            var settingsMock = A.Fake<IParsingSettings>();
            A.CallTo(() => settingsMock.IsPathFilteringEnabled).Returns(true);
            A.CallTo(() => settingsMock.PathRules)
                .Returns(testCase.PathRules);
            var testee = new SegmentDataCollector(settingsMock);

            // Act
            for (var i = 0; i < testCase.AddChecks.Count; i++)
            {
                var addCheck = testCase.AddChecks[i];
                testee.Add(addCheck.Path, addCheck.Value);

                // Assert
                if (addCheck.ExpectedSegmentData == null)
                {
                    testee.CompleteSegmentData.Should().BeNull($"because AddCheck #{i} expected segment data is null");
                }
                else
                {
                    testee.CompleteSegmentData.Should()
                        .NotBeNull($"because AddCheck #{i} expected segment data is not null");
                    testee.CompleteSegmentData.SourcePath.Should()
                        .Be(addCheck.ExpectedSegmentData.SourcePath,
                            $"because AddCheck #{i} expected segment SourcePath");
                    testee.CompleteSegmentData.SourceValue.Should()
                        .Be(addCheck.ExpectedSegmentData.SourceValue,
                            $"because AddCheck #{i} expected segment SourceValue");
                    testee.CompleteSegmentData.TargetPath.Should()
                        .Be(addCheck.ExpectedSegmentData.TargetPath,
                            $"because AddCheck #{i} expected segment TargetPath");
                    testee.CompleteSegmentData.TargetValue.Should()
                        .Be(addCheck.ExpectedSegmentData.TargetValue,
                            $"because AddCheck #{i} expected segment TargetValue");
                }
            }
        }

        [Test]
        public void Add_WhenPathFilteringIsNotEnabled_ShouldSetCompleteSegmentData()
        {
            // Arrange
            var settingsMock = A.Fake<IParsingSettings>();
            A.CallTo(() => settingsMock.IsPathFilteringEnabled).Returns(false);
            var testee = new SegmentDataCollector(settingsMock);

            // Act
            testee.Add("The test path", "The test value");

            // Assert
            testee.CompleteSegmentData.Should().NotBeNull();
            testee.CompleteSegmentData.SourcePath.Should().Be("The test path");
            testee.CompleteSegmentData.SourceValue.Should().Be("The test value");
            testee.CompleteSegmentData.TargetPath.Should().Be("The test path");
            testee.CompleteSegmentData.TargetValue.Should().Be("The test value");
        }
    }

    public class TestCase
    {
        public string Name { get; set; }

        public ComplexObservableList<PathRule> PathRules { get; set; }

        public List<AddCheck> AddChecks { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class AddCheck
    {
        public string Path { get; set; }

        public string Value { get; set; }

        public SegmentData ExpectedSegmentData { get; set; }
    }
}