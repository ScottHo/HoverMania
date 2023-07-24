using System;

public class SampleFactory
{
	public static Sample createSample(int id, int quantity=0)
	{
		Sample sample = new Sample();
		if (id == 0)
		{
			sample.id = 0;
			sample.value = 1;
			sample.rarity = 1;
			sample.name = "Common Rock Sample";
			sample.description = "A common rock sample found in many terrestial planets";
			sample.unique = false;
			sample.quantity = quantity;
		}
		return sample;
	}
}
