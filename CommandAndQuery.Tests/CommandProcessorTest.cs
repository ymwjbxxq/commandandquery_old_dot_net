using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandAndQuery.Commands;
using CommandAndQuery.Commands.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CommandAndQuery.Tests
{
    [TestClass]
    public class CommandProcessorTest
    {
        public class MyCommandRequest : ICommand
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        public class MyCommandResult : CommandResult
        {
        }


        private ICommandProcessor _classUndertest;
        private Mock<IServiceLocator> _serviceLocator;
        private Mock<ICommandHandler<MyCommandRequest, MyCommandResult>> _commandHandler;
        private Mock<ICommandValidatorFor<MyCommandRequest>> _commandValidator1;
        private Mock<ICommandValidatorFor<MyCommandRequest>> _commandValidator2;

        [TestInitialize]
        public void Init()
        {
            _serviceLocator = new Mock<IServiceLocator>();
            _commandValidator1 = new Mock<ICommandValidatorFor<MyCommandRequest>>();
            _commandValidator2 = new Mock<ICommandValidatorFor<MyCommandRequest>>();
            _commandHandler = new Mock<ICommandHandler<MyCommandRequest, MyCommandResult>>();

            _classUndertest = new CommandProcessor(_serviceLocator.Object);
        }

        [TestMethod]
        public async Task GIVEN_a_command_request_with_no_validators_WHEN_execute_THEN_we_return_command_result()
        {
            // Arrange
            var commandResult = new MyCommandResult();
            var command = new MyCommandRequest();

            _serviceLocator
                .Setup(x => x.Resolve<ICommandHandler<MyCommandRequest, MyCommandResult>>())
                .Returns(_commandHandler.Object);

            _serviceLocator
                .Setup(x => x.ResolveAll< ICommandValidatorFor<MyCommandRequest>>())
                .Returns(Enumerable.Empty<ICommandValidatorFor<MyCommandRequest>>().ToList());

            _commandHandler
                .Setup(x => x.Handle(command))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _classUndertest.Process<MyCommandRequest, MyCommandResult>(command);

            // Assert
            Assert.AreEqual(result, commandResult);
        }

        [TestMethod]
        public async Task GIVEN_a_command_request_with_2_failed_validators_WHEN_execute_THEN_we_return_command_result_with_all_errors()
        {
            // Arrange
            var commandResult = new MyCommandResult();
            var command = new MyCommandRequest();

            _serviceLocator
                .Setup(x => x.Resolve<ICommandHandler<MyCommandRequest, MyCommandResult>>())
                .Returns(_commandHandler.Object);

            _serviceLocator
                .Setup(x => x.ResolveAll<ICommandValidatorFor<MyCommandRequest>>())
                .Returns(new List<ICommandValidatorFor<MyCommandRequest>>()
                {
                    _commandValidator1.Object,
                    _commandValidator2.Object
                });

            _commandValidator1
                .Setup(x => x.Validate(command))
                .Returns(new ValidationResponse
                {
                    Errors = new[] { "error1" }
                });

            _commandValidator2
               .Setup(x => x.Validate(command))
               .Returns(new ValidationResponse
               {
                   Errors = new[] { "error2" }
               });

            _commandHandler
                .Setup(x => x.Handle(command))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _classUndertest.Process<MyCommandRequest, MyCommandResult>(command);

            // Assert
            Assert.AreEqual(2, result.ValidationResult.Errors.Count());
        }

        [TestMethod]
        public async Task GIVEN_a_command_request_with_only_1_failed_validator_WHEN_execute_THEN_we_return_command_result_with_1_error()
        {
            // Arrange
            var commandResult = new MyCommandResult();
            var command = new MyCommandRequest();

            _serviceLocator
                .Setup(x => x.Resolve<ICommandHandler<MyCommandRequest, MyCommandResult>>())
                .Returns(_commandHandler.Object);

            _serviceLocator
                .Setup(x => x.ResolveAll<ICommandValidatorFor<MyCommandRequest>>())
                .Returns(new List<ICommandValidatorFor<MyCommandRequest>>()
                {
                    _commandValidator1.Object,
                    _commandValidator2.Object
                });

            _commandValidator1
                .Setup(x => x.Validate(command))
                .Returns(new ValidationResponse
                {
                    Errors = new[] { "error1" }
                });

            _commandValidator2
               .Setup(x => x.Validate(command))
               .Returns(new ValidationResponse
               {
                   Errors = new string[] { }
               });

            _commandHandler
                .Setup(x => x.Handle(command))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _classUndertest.Process<MyCommandRequest, MyCommandResult>(command);

            // Assert
            Assert.AreEqual(1, result.ValidationResult.Errors.Count());
        }

        [TestMethod]
        public async Task GIVEN_a_command_request_WHEN_execute_THEN_we_execute_command_and_forget()
        {
            // Arrange
            var command = new MyCommandRequest();
            var commandHandler = new Mock<ICommandHandler<MyCommandRequest>>();

            _serviceLocator
                .Setup(x => x.Resolve<ICommandHandler<MyCommandRequest>>())
                .Returns(commandHandler.Object);

            // Act
            await _classUndertest.Process(command);

            // Assert
            commandHandler.Verify(x => x.Handle(command), Times.Once);
        }
    }
}
