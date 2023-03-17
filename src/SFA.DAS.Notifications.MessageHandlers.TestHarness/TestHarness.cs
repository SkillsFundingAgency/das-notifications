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
                Console.WriteLine("B - SendSmsCommand");
                Console.WriteLine("X - Exit");
                Console.WriteLine("Press [Key] for Test Option");
                key = Console.ReadKey().Key;

                try
                {
                    var tokenDictionary = new Dictionary<string, string>
                    {
                        { "cohort_reference", "MYREF1" }
                    };

                    var attachmentDictionary = new Dictionary<string, byte[]>
                    {
                        { "file_reference", new byte[10] }
                    };

                    switch (key)
                    {
                        case ConsoleKey.A:
                            var readOnlyDictionary = new ReadOnlyDictionary<string, string>(tokenDictionary);

                            await _publisher.Send(new SendEmailCommand("EmployerCohortApproved", "test@test.co.uk", readOnlyDictionary));
                            Console.WriteLine();
                            Console.WriteLine($"Sent SendEmailCommand");
                            break;

                        case ConsoleKey.B:
                            await _publisher.Send(new SendSmsCommand("EmployerCohortApproved", "test@test.co.uk", "07123456789", tokenDictionary));
                            Console.WriteLine();
                            Console.WriteLine($"Sent SendSmsCommand");
                            break;

                        case ConsoleKey.C:
                            var readOnlyAttachmentDictionary = new ReadOnlyDictionary<string, byte[]>(attachmentDictionary);
                            var dataBusProp = new ReadOnlyDictionary<string, byte[]>(readOnlyAttachmentDictionary);
                            await _publisher.Send(new SendEmailWithAttachmentsCommand("EmployerCohortApproved", "test@test.co.uk", tokenDictionary, dataBusProp));
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