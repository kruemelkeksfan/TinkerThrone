<?php
abstract class InputHelper
	{
	// Unsafe Method which requires checking afterwards
	public static function get_get_raw(string $name, $default = false)
		{
		return (isset($_GET[$name])) ? $_GET[$name] : $default;
		}
		
	public static function get_get_string(string $name, $default = false)
		{
		return (isset($_GET[$name])) ? htmlspecialchars($_GET[$name]) : $default;
		}
		
	public static function get_get_int(string $name, $default = false)
		{
		if(isset($_GET[$name]))
			{
			$input = htmlspecialchars($_GET[$name]);
			if(is_numeric($input))
				{
				return floor($input);
				}
			else
				{
				ErrorHandler::handle_warning('Received unexpected non-numeric GET Input ' . $input . '!');
				}
			}

		return $default;
		}
		
	public static function get_get_float(string $name, $default = false)
		{
		if(isset($_GET[$name]))
			{
			$input = htmlspecialchars($_GET[$name]);
			if(is_numeric($input))
				{
				return $input;
				}
			else
				{
				ErrorHandler::handle_warning('Received unexpected non-numeric GET Input ' . $input . '!');
				}
			}

		return $default;
		}	
	
	public static function get_get_bool(string $name, $default = false)
		{
		if(isset($_GET[$name]))
			{
			$input = htmlspecialchars($_GET[$name]);
			if($input === 'true' || $input === 'TRUE' || $input === '1' || $input === 'on')
				{
				return true;
				}
			else if($input === 'false' || $input === 'FALSE' || $input === '0' || $input === 'off')
				{
				return false;
				}
			else
				{
				ErrorHandler::handle_warning('Received unexpected non-boolean GET Input ' . $input . '!');
				}
			}

		return $default;
		}
	
	// Unsafe Method which requires checking afterwards
	public static function get_post_raw(string $name, $default = false)
		{
		return (isset($_POST[$name])) ? $_POST[$name] : $default;
		}
	
	public static function get_post_string(string $name, $default = false)
		{
		return (isset($_POST[$name])) ? htmlspecialchars($_POST[$name]) : $default;
		}
	
	public static function get_post_int(string $name, $default = false)
		{
		if(isset($_POST[$name]))
			{
			$input = htmlspecialchars($_POST[$name]);
			if(is_numeric($input))
				{
				return round($input);
				}
			else
				{
				ErrorHandler::handle_warning('Received unexpected non-numeric POST Input ' . $input . '!');
				}
			}

		return $default;
		}
	
	public static function get_post_float(string $name, $default = false)
		{
		if(isset($_POST[$name]))
			{
			$input = htmlspecialchars($_POST[$name]);
			if(is_numeric($input))
				{
				return $input;
				}
			else
				{
				ErrorHandler::handle_warning('Received unexpected non-numeric POST Input ' . $input . '!');
				}
			}

		return $default;
		}	

	public static function get_post_bool(string $name, $default = false)
		{
		if(isset($_POST[$name]))
			{
			$input = htmlspecialchars($_POST[$name]);
			if(strcasecmp($input, 'true'))
				{
				return true;
				}
			else if(strcasecmp($input, 'false'))
				{
				return false;
				}
			else
				{
				ErrorHandler::handle_warning('Received unexpected non-boolean POST Input ' . $input . '!');
				}
			}

		return $default;
		}
	}
?>