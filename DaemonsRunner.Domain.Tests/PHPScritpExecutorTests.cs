using DaemonsRunner.Domain.Exceptions.Base;
using DaemonsRunner.Domain.Models;
using DaemonsRunner.Domain.Tests.Infrastructure;
using DaemonsRunner.Domain.Tests.Infrastructure.EventSpies;

namespace DaemonsRunner.Domain.Tests
{
    public class PHPScritpExecutorTests : IClassFixture<TestStorageFixture>
    {
        private readonly TestStorageFixture _testStorage;

        public PHPScritpExecutorTests(TestStorageFixture testStorage) 
        {
            _testStorage = testStorage;
        }

        [Fact]
        public void IS_Object_Disposed_Exception_Thrown_on_disposed_object_propertiesUsing()
        {
            using var testObject = CreateTestExecutor();

            testObject.Dispose();

            Assert.Throws<ObjectDisposedException>(() => testObject.IsRunning);
            Assert.Throws<ObjectDisposedException>(() => testObject.IsMessagesReceiving);
            Assert.Throws<ObjectDisposedException>(() => testObject.ExecutableScript);
        }

        [Fact]
        public async Task IS_Object_Disposed_Exception_Thrown_on_disposed_object_methodsUsing()
        {
            using var testObject = CreateTestExecutor();

            testObject.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(testObject.StartAsync);
            await Assert.ThrowsAsync<ObjectDisposedException>(testObject.StartMessagesReceivingAsync);
            await Assert.ThrowsAsync<ObjectDisposedException>(testObject.ExecuteCommandAsync);
            await Assert.ThrowsAsync<ObjectDisposedException>(testObject.StopMessagesReceivingAsync);
            await Assert.ThrowsAsync<ObjectDisposedException>(testObject.StopAsync);
        }

        [Fact]
        public async Task IS_Starting_Correct()
        {
            bool expected = true;
            using var testObject = CreateTestExecutor();

            await testObject.StartAsync();

            Assert.Equal(expected, testObject.IsRunning);
        }

        [Fact]
        public async Task IS_Start_NOT_Available_when_try_to_start_already_running_executor()
        {
            using var testExecutor = CreateTestExecutor();

            var startingResult = await testExecutor.StartAsync();

            await Assert.ThrowsAsync<DomainException>(testExecutor.StartAsync);
        }

        [Fact]
        public async Task IS_Stopping_Correct()
        {
            bool expected = false;
            using var testObject = CreateTestExecutor();

            await testObject.StartAsync();
            await testObject.StopAsync();

            Assert.Equal(expected, testObject.IsRunning);
        }

        [Fact]
        public async Task IS_Stop_NOT_Available_when_try_to_stop_not_running_executor()
        {
            using var testExecutor = CreateTestExecutor();

            await Assert.ThrowsAsync<DomainException>(testExecutor.StopAsync);
        }

        [Fact]
        public async Task IS_Stop_NOT_Available_when_try_to_stop_but_messages_receiving_is_not_stopped()
        {
            using var testExecutor = CreateTestExecutor();

            await testExecutor.StartAsync();
            await testExecutor.StartMessagesReceivingAsync();

            await Assert.ThrowsAsync<DomainException>(testExecutor.StopAsync);
        }

        [Fact]
        public async Task IS_Messages_Receiving_Starting_Correct()
        {
            bool expected = true;
            using var testExecutor = CreateTestExecutor();

            await testExecutor.StartAsync();
            await testExecutor.StartMessagesReceivingAsync();

            Assert.Equal(expected, testExecutor.IsMessagesReceiving);
        }

        [Fact]
        public async Task IS_Messages_Receiving_NOT_Available_when_executor_was_not_started()
        {
            using var testExecutor = CreateTestExecutor();

            await Assert.ThrowsAsync<DomainException>(testExecutor.StartMessagesReceivingAsync);
        }

        [Fact]
        public async Task IS_Messages_Receiving_NOT_Available_when_messages_receiving_already_started()
        {
            using var testExecutor = CreateTestExecutor();

            await testExecutor.StartAsync();
            await testExecutor.StartMessagesReceivingAsync();

            await Assert.ThrowsAsync<DomainException>(testExecutor.StartMessagesReceivingAsync);
        }

        [Fact]
        public async Task IS_Command_Executing_NOT_Available_when_executor_NOT_started()
        {
            using var testExecutor = CreateTestExecutor();

            await Assert.ThrowsAsync<DomainException>(testExecutor.ExecuteCommandAsync);
        }

        [Fact]
        public async Task IS_Executor_Exited_By_Task_Manager_Event_NOT_Raised_when_kills_the_executor_by_task_manager()
        {
            using var testExecutor = CreateTestExecutor();
            var eventSpy = new ExecutorExitedByTaskManagerEventSpy();
            testExecutor.ExitedByTaskManager += eventSpy.HandleEvent;

            await testExecutor.StartAsync();
            await testExecutor.StopAsync();
            await Task.Delay(eventSpy.EventWaitTimeMs);

            Assert.False(eventSpy.EventHandled);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task IS_Executor_Can_Be_Reusable_when_only_start_and_stop_invoked(int reusingCycleCount)
        {
            using var testExecutor = CreateTestExecutor();

            for (int i = 0; i < reusingCycleCount; i++)
            {
                await testExecutor.StartAsync();
                Assert.True(testExecutor.IsRunning);
                await testExecutor.StopAsync();
                Assert.False(testExecutor.IsRunning);
            }
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task IS_Executor_Can_Be_Reusable_when_all_executor_methods_invoked(int reusingCycleCount)
        {
            using var testExecutor = CreateTestExecutor();

            for (int i = 0; i < reusingCycleCount; i++)
            {
                await testExecutor.StartAsync();
                Assert.True(testExecutor.IsRunning);

                await testExecutor.StartMessagesReceivingAsync();
                Assert.True(testExecutor.IsMessagesReceiving);

                await testExecutor.ExecuteCommandAsync();

                await testExecutor.StopMessagesReceivingAsync();
                Assert.False(testExecutor.IsMessagesReceiving);

                await testExecutor.StopAsync();
                Assert.False(testExecutor.IsRunning);
            }
        }

        private PHPScriptExecutor CreateTestExecutor()
        {
            var randomFileName = _testStorage.GetRandomFileName();
            var phpFile = PHPFile.Create(randomFileName, Path.Combine(_testStorage.TestDirectoryPath, randomFileName));
            var script = PHPScript.Create(phpFile);

            return PHPScriptExecutor.Create(script);
        }
    }
}
