///
/// This script was taken from a separate project Elijah Shadbolt worked on.
/// It was copied over 03/06/2021.
///

/// <changelog>
///		<log author="Elijah Shadbolt" date="10/02/2021">
///			Copied this script from Prototype 1 of Compassionate Design.
///		</log>
///		<log author="Elijah Shadbolt" date="16/02/2021">
///			Edited script for readability.
///		</log>
/// </changelog>
/// 

using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

/// <summary>
/// A way of performing asynchronous tasks in series without blocking Unity's thread,
/// and still being able to detect (from Unity's thread) when each asynchronous task is done.
/// </summary>
public abstract class WorkQueueComponent : MonoBehaviour
{
	protected virtual void OnWorkDone(object result)
	{
		DefaultOnWorkDone(result);
	}

	protected void DefaultOnWorkDone(object result)
	{
		if (result is Exception exception)
		{
			Debug.LogException(exception, this);
			return;
		}

		Debug.LogError("Thread returned unexpected value.", this);
	}

	protected void EnqueueWork<A>(A immutableData, StartAsyncTaskFunc<A> startAsyncTask)
	{
		var work = new QueuedTask<A>(immutableData, startAsyncTask);

		if (isDoingWork)
		{
			taskQueue.Enqueue(work);
			return;
		}

		SpawnWorker(work);
	}

	protected void EnqueueWork(StartAsyncTaskFunc startAsyncTask)
	{
		EnqueueWork(default(Voidlike), _ => startAsyncTask());
	}

	#region Types

	public delegate Task<object> StartAsyncTaskFunc<A>(A immutableData);
	public delegate Task<object> StartAsyncTaskFunc();

	private struct Voidlike { }

	private interface IQueuedTask
	{
		Task<object> StartAsyncTask();
	}

	private class QueuedTask<A> : IQueuedTask
	{
		public QueuedTask(A immutableData, StartAsyncTaskFunc<A> startAsyncTask)
		{
			this.immutableData = immutableData;
			this.startAsyncTask = startAsyncTask;
		}

		public A immutableData { get; private set; }
		public StartAsyncTaskFunc<A> startAsyncTask { get; private set; }

		public Task<object> StartAsyncTask() => startAsyncTask(immutableData);
	}


	private class CallbackData<T>
	{
		public CallbackData(
			ConcurrentQueue<object> taskResultQueue,
			T extraData
			)
		{
			this.taskResultQueue = taskResultQueue;
			this.extraData = extraData;
		}

		public ConcurrentQueue<object> taskResultQueue { get; private set; }
		public T extraData { get; private set; }
	}

	#endregion

	public bool isDoingWork { get; private set; }

	private Queue<IQueuedTask> taskQueue = new Queue<IQueuedTask>();

	private ConcurrentQueue<object> taskResultQueue = new ConcurrentQueue<object>();



	private void SpawnWorker(IQueuedTask work)
	{
		isDoingWork = true;
		_ = RunWorkerAsync(work, this.taskResultQueue);
	}

	private static async Task RunWorkerAsync(
		IQueuedTask work,
		ConcurrentQueue<object> resultQueue)
	{
		try
		{
			var result = await work.StartAsyncTask();
			resultQueue.Enqueue(result);
		}
		catch (Exception e)
		{
			resultQueue.Enqueue(e);
		}
	}

	protected virtual void Update()
	{
		if (taskResultQueue.TryDequeue(out var result))
		{
			HandleWorkDone(result);
		}
	}

	private void HandleWorkDone(object result)
	{
		isDoingWork = false;
		OnWorkDone(result);
		TrySpawnNextTask();
	}

	private void TrySpawnNextTask()
	{
		if (taskQueue.Count <= 0)
			return;

		var queuedTask = taskQueue.Dequeue();
		SpawnWorker(queuedTask);
	}
}