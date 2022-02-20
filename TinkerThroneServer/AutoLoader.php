<?php
class AutoLoader
	{
	private $prefixes;
	
	function __construct()
		{
		$this->prefixes = array('');
		
		spl_autoload_register(array($this, 'load'));
		}
		
	function load($classname)
		{
		foreach($this->prefixes as $prefix)
			{
			$path = $prefix . $classname . '.php';
			if(is_readable($path))
				{
				include($path);
				return;
				}		
			}
			
		var_dump($classname . '.php could not be found in any known Folder!');
		var_dump('Searched in: ');
		foreach($this->prefixes as $prefix)
			{
			var_dump($prefix . $classname . '.php');	
			}
		}
	}

?>