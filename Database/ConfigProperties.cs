namespace OpenRobo.Database;

public class ConfigType
{
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class TextBoxAttribute : System.Attribute { }
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class NumberBoxAttribute : System.Attribute { }
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class ChannelSelection : System.Attribute { }
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class ChannelList : System.Attribute { }
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class RoleSelectionAttribute : System.Attribute { }
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class IntAndRoleSelectionAttribute : System.Attribute { }
}
