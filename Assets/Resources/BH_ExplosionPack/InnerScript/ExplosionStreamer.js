#pragma strict
#pragma implicit
#pragma downcast

var emitters : ParticleEmitter[];var minSpeed = 0.00;var maxSpeed = 0.00;var drag = 0.00;var randomForce = 0.00;var drop = 0.00;
var collisionDamping = 0.00;var emitTimeOut = 0.00;var shrinkTime = 0.00;var timeRandom = 0.00;var minSize = 0.00;private var velocity : Vector3;private var random : Vector3;private var timer = 0.00;function Start (){	random = Random.insideUnitSphere * randomForce;	velocity = transform.TransformDirection(Vector3.forward) * Random.Range(minSpeed, maxSpeed);	r = Random.Range(-timeRandom, timeRandom);	emitTimeOut += r;}function FixedUpdate () {	velocity.y -= drop * Time.fixedDeltaTime;	velocity += random * Time.fixedDeltaTime;	velocity -= velocity * drag * Time.fixedDeltaTime;	for(e in emitters)	{
		if(e.minSize > 0.02)
		{			e.minSize = e.minSize * 1 - (timer / (shrinkTime));			e.maxSize = e.maxSize * 1 - (timer / (shrinkTime));
		}
				e.rndVelocity *= 1 - (timer / (emitTimeOut * 2));
		
		if(timer > emitTimeOut || e.maxSize < minSize) e.emit = false;	}
	
	var hit : RaycastHit;
	if(Physics.Raycast(transform.position, velocity, hit, velocity.magnitude))
	{
		velocity = Vector3.Reflect(velocity, hit.normal) * collisionDamping;
	}
		transform.position += velocity * Time.fixedDeltaTime;		if(timer > emitTimeOut)	{		transform.DetachChildren();		Destroy(gameObject);	}		timer += Time.fixedDeltaTime;}