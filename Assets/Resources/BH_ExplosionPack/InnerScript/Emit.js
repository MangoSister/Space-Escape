#pragma strict
#pragma implicit
#pragma downcast

var time = 0.00;
private var done = false;

function Update ()
{
	if(!done)
	{
		done = true;
		if(time != 0) Wait();
		else GetComponent.<ParticleEmitter>().emit = true;
	}
}

function Wait()
{
	yield WaitForSeconds (time);
	GetComponent.<ParticleEmitter>().emit = true;
}