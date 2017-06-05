using System;

namespace Mkafeina.Server.Domain.Entities
{
	public enum OrderStatusEnum
	{
		Undef = 0,
		InQueue,
		Taken,
		BeingProcessed,
		Ready
	}

	public class Order
	{
		public string RecipeName { get; set; }

		public uint Reference { get; set; }

		public string CustomerEmail { get; set; }

		public OrderStatusEnum Status { get; set; }

		public DateTime CreationTime { get; set; }

		public DateTime TakenTime { get; set; }

		public DateTime ReadyTime { get; set; }

		public DateTime StartedTime { get; set; }
	}
}