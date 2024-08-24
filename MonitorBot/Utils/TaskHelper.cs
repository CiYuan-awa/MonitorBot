namespace MonitorBot.Utils
{
	public static class TaskHelper
	{
		public static async Task Catch(this Task task)
		{
			try
			{
				await task;
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
			}
		}

		public static async Task<T?> Catch<T, TTask>(this Task<TTask> task)
			where T : class
			where TTask : T
		{
			try
			{
				return await task;
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
			}

			return null;
		}

		public static async Task<T> Catch<T, TTask>(this Task<TTask> task, T defaultValue)
			where T : class
			where TTask : T
		{
			try
			{
				return await task;
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
			}

			return defaultValue;
		}

		public static async Task<T?> Catch<T>(this Task<T> task)
			where T : struct
		{
			try
			{
				return await task;
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
			}

			return null;
		}

		public static async Task<T> Catch<T>(this Task<T> task, T defaultValue)
			where T : struct
		{
			try
			{
				return await task;
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
			}

			return defaultValue;
		}

		public static async Task<T> Catch<T, TTask>(this Task<TTask> task, Func<T> defaultValue)
			where TTask : T
		{
			try
			{
				return await task;
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
			}

			return defaultValue.Invoke();
		}

		public static void Catch(this Func<Task> action)
		{
			try
			{
				action();
			}
			catch (Exception exception)
			{
				Logger.Error(exception);
			}
		}

		public static void Ignore(this Task task)
		{
		}

		public static void Ignore<TTask>(this Task<TTask> task)
		{
		}

		public static Task _(this int value)
		{
			return Task.CompletedTask;
		}
	}
}