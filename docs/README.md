# CallTracing
[![NuGet version (CallTracing)](https://img.shields.io/nuget/v/CallTracing.svg?style=flat-square)](https://www.nuget.org/packages/CallTracing/)

## Purpose 

This library was developed with the aim to track mock calls within unit tests. Although other use cases also can be discovered.

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

Notice that `BoxMock` takes a `CallTrace` instance in the constructor and then uses it in its methods bodies to designate that a method or a property was called.

Then let's create a factory method for `WriteLog` delegate:

```C#
static WriteLog CreateWriteLog(CallTrace callTrace)
{
  return (string message) =>
  {
      callTrace.Add((WriteLog writeLog) => writeLog(message));
  };
}
```

Similarly to `BoxMock` the factory method takes a `CallTrace` instance and then it's used to designate that a delegate was called. 

Now the test method to assert that right calls and in the right order were invoked can be defined in following way:

```C#
void Test()
{
  var callTraceActual = new CallTrace();
  
  var boxMock = new BoxMock(callTraceActual);
  var writeLogMock = CreateWriteLog(callTraceActual);

  var things = new object[] { 1, 2 };
  FillBox(boxMock, things, writeLogMock);

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







