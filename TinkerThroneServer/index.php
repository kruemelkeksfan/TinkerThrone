<?php
// DEBUG
/*echo('Method Name: ' . $_POST['MethodName'] . ' Parameters: ');
foreach($_POST as $k => $v)
{
	echo($k . ' => ' . $_POST[$k] . ' ');
}*/

// Setup AutoLoader
include_once('AutoLoader.php');
new AutoLoader();

if(empty($_POST['MethodName']))
{
	echo('This site is not meant for you, meager human');
}

// Init Database
$database = new Database();
		
// PHP Settings
ini_set('session.use_strict_mode', '1');
ini_set('max_execution_time', 10);
ignore_user_abort(true);
		
// Start Session
session_start();

$response = '';

if($_POST['MethodName'] === 'Register')
{
	$username = InputHelper::get_post_string('Username', '');
	$password = InputHelper::get_post_string('Password', '');
	$repeat_password = InputHelper::get_post_string('RepeatPassword', '');
	
	if(!empty($username) && !empty($password) && !empty($repeat_password))
	{
		if($password === $repeat_password)
		{
			if($database->query('INSERT INTO Users (username, password) VALUES (:0, :1);',
				array($username, password_hash($password, PASSWORD_DEFAULT))))
			{
				$_SESSION['username'] = $username;

				$response .= 'Successful|';
			}
			else
			{
				$response .= 'Username in Use|';
				$response .= 'Username=' . $username . '|';
			}
		}
		else
		{
			$response .= 'Passwords do not match|';
			$response .= 'Password=' . $password . '|';
			$response .= 'RepeatPassword=' . $repeat_password . '|';
		}
	}
	else
	{
		$response .= 'Incomplete Data|';
		$response .= 'Username=' . $username . '|';
		$response .= 'Password=' . $password . '|';
		$response .= 'RepeatPassword=' . $repeat_password . '|';
	}
}
else if($_POST['MethodName'] === 'Login')
{
	$username = InputHelper::get_post_string('Username', '');
	$password = InputHelper::get_post_string('Password', '');

	if(!empty($username) && !empty($password))
	{
		$userdata = $database->query('SELECT password FROM Users WHERE username=:0;', array($username));

		if(count($userdata) > 0 && password_verify($password, $userdata[0]['password']))
		{
			$_SESSION['username'] = $username;

			$response .= 'Successful|';
		}
		else
		{
			$response .= 'Invalid Password|';
			$response .= 'Username=' . $username . '|';
			$response .= 'Password=' . $password . '|';
		}
	}
	else
	{
		$response .= 'Incomplete Data|';
		$response .= 'Username=' . $username . '|';
		$response .= 'Password=' . $password . '|';
	}
}
else if($_POST['MethodName'] === 'Logout')
{
	unset($_SESSION['username']);
	session_destroy();

	$response .= 'Successful|';
}
else
{
	$response .= 'Unknown Method Name on Server ' . $_POST['MethodName'] . '|';
}

echo($_POST['MethodName'] . ':' . $response);
?>