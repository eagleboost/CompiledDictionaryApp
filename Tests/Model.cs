namespace Tests;

public class Model(int id)
{
  public readonly int Id = id;

  public override string ToString() => Id.ToString();
    
  public override int GetHashCode()
  {
    return Id.GetHashCode();
  }
}