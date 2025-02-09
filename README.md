# RandomSizesZA 1.2.27.2
 7 Days to die 1.2.27
 Random Sizes for Zombies and Animals
 Network synced communication using custom monobehaviour net packages
 Sync sizes from server to clients
 Works single player, non-dedicated and dedicated servers
 Error checking
 Null Ref checking
 DebugMode for very verbose logging, useful for patching future versions of the game

 Many hours went into this.

Roadmap:
 Set health and attack power based on size by a factor of 10x per 10% change in size.
 Impose limits on sizes
 
<!-- FEATURES -->
		<!-- 
			Decide which features to use. All enabled by default

			true = randomize sizes
			false = dont randomize sizes
		-->

		<randomZombieSizes>true</randomZombieSizes>

		<randomAnimalSizes>true</randomAnimalSizes>
		

	<!-- Randomize sizes -->
		<!-- 
			Decide Min and Max to use.

			use percentage decimals, where 1.0 means regular size
			0.5 means half size, and 2.0 means double size
			Min must be less than or equal to Max
			Values must be above zero
			
			Defaults: 0.75, 1.25
		-->

		<!-- Zombie Min/Max size -->
		<zombieMin>0.75</zombieMin>

		<zombieMax>1.25</zombieMax>


		<!-- Animal Min/Max size -->
		<animalMin>0.75</animalMin>

		<animalMax>1.25</animalMax>


	<!-- Debug, dont use -->
		<debugMode>false</debugMode>>
