# CallTracing
[![NuGet version (CallTracing)](https://img.shields.io/nuget/v/CallTracing.svg)](https://www.nuget.org/packages/CallTracing/)

## Purpose 

This library was developed with the aim to track mock calls in unit tests for .NET. Although other use cases could be discovered.

## Motivation

Some mocking  libraries have similar functionality. For example Moq library has [MockSequence](https://github.com/Moq/moq4/wiki/Quickstart#miscellaneous). But there was a desire to have this functionality without being dependent on the entire mocking library and its peculiarities of usage. 

## Example of usage

Let’s have the following interface and delegate. The interface defines a `box` which can be opened, closed, and things can be put into and counted. The purpose of the delegate is to write logs:

```C#
interface IBox
{
  int Count { get; }
  void Open();
  void PutInto(object thing);
  void Close();
}

delegate void WriteLog(string message);
```

And let's have the following method which should be tested:

```C#
static void FillBox(
  IBox box, 
  IEnumerable<object> things, 
  WriteLog writeLog)
{
    box.Open();
    writeLog("The box is opened.");

    foreach (var thing in things)
    {
        box.PutInto(thing);
    }

    writeLog($"The box has {box.Count} things.");

    box.Close();
    writeLog("The box is closed.");
}
```

The test should ensure that `FillBox` makes all necessary calls to `IBox` and `WriteLog` and does it in proper order.

Before writing the test let's create a mock for `IBox` first:
```C#
class BoxMock : IBox
{
  private readonly CallTrace callTrace;

  public BoxMock(CallTrace callTrace)
  {
      this.callTrace = callTrace;
  }

  public int Count
  {
      get
      {
          callTrace.Add((IBox box) => box.Count);
          return 2;
      }
  }

  public void Open()
  {
      callTrace.Add((IBox box) => box.Open());
  }

  public void PutInto(object thing)
  {
      callTrace.Add((IBox box) => box.PutInto(thing));
  }

  public void Close()
  {
      callTrace.Add((IBox box) => box.Close());
  }
}
```

Notice that `BoxMock` takes a `CallTrace` instance in the constructor and then uses it in its methods and properties to designate that the method or the property was called.

Then let's create a factory method for `WriteLog` delegate mock:

```C#
static WriteLog CreateWriteLogMock(CallTrace callTrace)
{
  return (string message) =>
  {
      callTrace.Add((WriteLog writeLog) => writeLog(message));
  };
}
```

Similarly to `BoxMock` the factory method takes a `CallTrace` instance and then it's used to designate that the delegate was called. 

Now the test method to assert that right calls and in the right order were invoked can be implemented in following way:

```C#
static void Test()
{
  var callTraceActual = new CallTrace();
  
  var box = new BoxMock(callTraceActual);
  var writeLog = CreateWriteLogMock(callTraceActual);

  var things = new object[] { 1, 2 };
  FillBox(box, things, writeLog);

  var callTraceExpected = new CallTrace(
      (IBox box) => box.Open(),
      (WriteLog writeLog) => writeLog("The box is opened."),
      (IBox box) => box.PutInto(1),
      (IBox box) => box.PutInto(2),
      (IBox box) => box.Count,
      (WriteLog writeLog) => writeLog("The box has 2 things."),
      (IBox box) => box.Close(),
      (WriteLog writeLog) => writeLog("The box is closed."));

  Assert.Equal(callTraceExpected, callTraceActual);
}
```

## Documentation
The example above demonstrates pretty much the entire functionality of the library.







