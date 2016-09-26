using System.Collections.Generic;
using System.IO;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using FlashDevelop;
using NSubstitute;
using NUnit.Framework;
using PluginCore;
using PluginCore.Helpers;

namespace HaXeContext
{
    [TestFixture]
    public class ContextTests
    {
        [Test]
        public void ResolveType_HaxeGenerycType()
        {
            PluginBase.Initialize(new MainForm());
            PlatformData.Load(Path.Combine(PathHelper.SettingDir, "Platforms"));
            var plugin = Substitute.For<ASCompletion.PluginMain>();
            plugin.Settings.Returns(new GeneralSettings());
            ASContext.GlobalInit(plugin);
            var context = new Context(new HaXeSettings());
            context.BuildClassPath();

            var dummyClass = new ClassModel();
            dummyClass.Name = "DummyClass";
            dummyClass.Type = "org.DummyClass.DummyClass<K:IImaginary, V>";
            dummyClass.Template = "<K:IImaginary, V>";
            dummyClass.InFile = new FileModel {Package = "org"};
            dummyClass.ExtendsType = "org.BaseDummyClass<K, V>";
            dummyClass.Implements = new List<string> { "IClass<K>", "IClass2<V>", "IDummyClass<K, V>" };
            var dummyClassTestMethod = new MemberModel("getValue", "V", FlagType.Function, Visibility.Public);
            var dummyClassTestMethod2 = new MemberModel("getValues", "Array<V>", FlagType.Function, Visibility.Public);
            dummyClassTestMethod.Parameters = new List<MemberModel>(new[]{ new MemberModel {Name = "key", Type = "K"} });
            dummyClass.Members.Add(dummyClassTestMethod);
            dummyClass.Members.Add(dummyClassTestMethod2);

            var fileModel = new FileModel();
            fileModel.Classes.Add(dummyClass);

            var specificClass = context.ResolveType("DummyClass<String, test.DummyClass<String, {r:Dynamic}>>", fileModel);
            Assert.AreEqual("String, test.DummyClass<String, {r:Dynamic}>", specificClass.IndexType);
            Assert.AreEqual("DummyClass<String, test.DummyClass<String, {r:Dynamic}>>", specificClass.Name);
            Assert.AreEqual("IClass<String>", specificClass.Implements[0]);
            Assert.AreEqual("IClass2<test.DummyClass<String, {r:Dynamic}>>", specificClass.Implements[1]);
            Assert.AreEqual("IDummyClass<String, test.DummyClass<String, {r:Dynamic}>>", specificClass.Implements[2]);
            Assert.AreEqual("test.DummyClass<String, {r:Dynamic}>", specificClass.Members[0].Type);
            Assert.AreEqual("String", specificClass.Members[0].Parameters[0].Type);
            Assert.AreEqual("Array<test.DummyClass<String, {r:Dynamic}>>", specificClass.Members[1].Type);
        }

        [Test]
        public void ResolveType_HaxeProxyType()
        {
            PluginBase.Initialize(new MainForm());
            PlatformData.Load(Path.Combine(PathHelper.SettingDir, "Platforms"));
            var plugin = Substitute.For<ASCompletion.PluginMain>();
            plugin.Settings.Returns(new GeneralSettings());
            ASContext.GlobalInit(plugin);
            var context = new Context(new HaXeSettings());
            context.BuildClassPath();

            var proxyClass = new ClassModel();
            proxyClass.Name = "Proxy";
            proxyClass.Type = "haxe.remoting.Proxy.Proxy<T>";
            proxyClass.Template = "<T>";
            proxyClass.InFile = new FileModel { Package = "haxe.remoting", FullPackage = "haxe.remoting.Proxy"};

            proxyClass.InFile.Classes.Add(proxyClass);

            context.Classpath[0].AddFile(proxyClass.InFile);

            var specificClass = context.ResolveType("haxe.remoting.Proxy.Proxy<DummyClass>", new FileModel());
            Assert.AreEqual("DummyClass", specificClass.ExtendsType);
        }
    }
}