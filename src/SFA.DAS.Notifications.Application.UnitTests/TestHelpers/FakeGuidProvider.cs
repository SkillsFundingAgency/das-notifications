using System;
using SFA.DAS.Notifications.Application.Services;

namespace SFA.DAS.Notifications.Application.UnitTests.TestHelpers
{
    public class FakeGuidProvider : GuidProvider
    {
        private readonly Guid _guid;

        public FakeGuidProvider(Guid guid)
        {
            _guid = guid;
        }

        public override Guid NewGuid()
        {
            return _guid;
        }
    }
}