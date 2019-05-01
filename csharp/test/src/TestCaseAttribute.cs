using System;

namespace PhysFS_CS_Test {
    public class TestCaseAttribute : Attribute {
        public TestCaseAttribute(string title) {
            Title = title;
        }

        public string Title { get; private set; }
    }
}
