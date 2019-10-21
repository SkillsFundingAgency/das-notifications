using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.Notifications.MessageHandlers.TestHarness
{
    public class TestHarness
    {
        private readonly IMessageSession _publisher;

        public TestHarness(IMessageSession publisher)
        {
            _publisher = publisher;
        }

        public async Task Run()
        {

            ConsoleKey key = ConsoleKey.Escape;

            while (key != ConsoleKey.X)
            {
                Console.Clear();
                Console.WriteLine("Test Options");
                Console.WriteLine("------------");
                Console.WriteLine("A - SendEmailCommand");
                Console.WriteLine("X - Exit");
                Console.WriteLine("Press [Key] for Test Option");
                key = Console.ReadKey().Key;


                try
                {
                    switch (key)
                    {
                        case ConsoleKey.A:

                            var dictionary = new Dictionary<string, string>();
                            dictionary.Add("cohort_reference", "MYREF1");

                            var readOnlyDictionary = new ReadOnlyDictionary<string, string>(dictionary);

                            await _publisher.Send(new SendEmailCommand("ProviderCohortApproved", "paul.graham@coprime.co.uk", "noreply@sfa.gov.uk", readOnlyDictionary));
                            Console.WriteLine();
                            Console.WriteLine($"Sent SendEmailCommand");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                }

                if (key == ConsoleKey.X) break;

                Console.WriteLine();
                Console.WriteLine("Press anykey to return to menu");
                Console.ReadKey();
            }
        }
    }
}
