﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Proximo.Cqrs.Bus.RhinoEsb.Commanding;
using Rhino.ServiceBus;
using Rhino.ServiceBus.Impl;
using Sample.Commands.Inventory;
using Proximo.Cqrs.Core.Commanding;
using Sample.Commands.Purchases;
using log4net.Config;
using Proximo.Cqrs.Bus.RhinoEsb.Castle;
using Sample.Commands.System;

namespace Sample.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			XmlConfigurator.Configure();
			//
			// setup
			//
			var container = new WindsorContainer();

			container.Install(
				new OnewayRhinoServiceBusInstaller()
				);

			container.Register(Component.For<ICommandQueue>().ImplementedBy<RhinoEsbOneWayCommandQueue>());
            //
            // Enqueue command
            //
            var commandSender = container.Resolve<ICommandQueue>();

			//Console.WriteLine("Client ready");

            ////TODO: do not send directly a poisoningcommand in client until an handler is ready do handle it.
            //commandSender.Enqueue(new PoisoningCommand(Guid.NewGuid()));

			//
			// Create command
			//
			
			//var id = Guid.NewGuid();
			//ICommand command = new CreateInventoryItemCommand(id)
			//                  {
			//                      ItemId = id,
			//                      Sku = "I001",
			//                      Description = "New Item from client"
			//                  };


			//commandSender.Enqueue(command);
			//Console.WriteLine("Issued new Item Command");
             
/*
			// 
			// create an update command
			//
			command = new UpdateInventoryItemDescriptionCommand(Guid.NewGuid())
			{
				ItemId = id,
				Description = "Updated Item description"
			};
			commandQueue.Enqueue(command);
			Console.WriteLine("Issued update Item description Command");

			// ask to replay the events
			System.Threading.Thread.Sleep(3000);

			commandQueue.Enqueue(new AskForReplayCommand(Guid.NewGuid()));
			Console.WriteLine("Issued Ask For Replay Command");
*/			
            /*
			//
			// Bill of lading
			//
			RegisterBillOfLadingCommand receiveBoL = new RegisterBillOfLadingCommandBuilder(Guid.NewGuid())
				.From("Lucas Arts", "Somewhere")
				.IssuedAt(new DateTime(2012, 3, 12))
                .Numbered("001")
				    .AddRow(Guid.NewGuid(), "MI", "The Secret of Monkey Island", 1000)
				    .AddRow(Guid.NewGuid(), "ZAK", "Zak McKracken and the Alien Mindbenders", 1000)
				.Build();

			commandQueue.Enqueue(receiveBoL);
            Console.WriteLine("Received Bill of Lading");
			*/
            /*
            
			// ask to replay the events
			System.Threading.Thread.Sleep(4000);
			commandQueue.Enqueue(new AskForReplayCommand(Guid.NewGuid()));
			Console.WriteLine("Issued Ask For Replay Command");
            */

            // saga sample command sequence
            Guid correlatinId = Guid.NewGuid();
            Saga.DemoCommand1 c1 = new Saga.DemoCommand1()
            {
                Id = Guid.NewGuid(),
                AggregateId = Guid.NewGuid(),
                CorrelatonId = correlatinId
            };
            commandSender.Enqueue(c1);
            Console.WriteLine("issued Democommand1");
            Console.WriteLine("press a key to continue");
            Console.ReadLine();

            Saga.DemoCommand2 c2 = new Saga.DemoCommand2()
            {
                Id = Guid.NewGuid(),
                AggregateId = Guid.NewGuid(),
                CorrelatonId = correlatinId
            };
            commandSender.Enqueue(c2);
            Console.WriteLine("issued Democommand2");
            Console.WriteLine("press a key to continue");
            Console.ReadLine();

			//
			// shutdown
			//
			Console.ReadLine();
			container.Dispose();
		}
	}
}
