﻿namespace OpenRobo.Database;

public class ConfigType
{
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class TextBoxAttribute : System.Attribute { }
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class NumberBoxAttribute : System.Attribute { }
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class ChannelSelectionAttribute : System.Attribute { }
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class ChannelListAttribute : System.Attribute { }
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class RoleSelectionAttribute : System.Attribute { }
	[System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = true)]
	public class IntAndRoleListAttribute : System.Attribute { }
}
