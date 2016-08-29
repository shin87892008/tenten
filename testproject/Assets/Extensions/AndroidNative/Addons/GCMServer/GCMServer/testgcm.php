<?php


// API access key from Google API's Console
//define( 'API_ACCESS_KEY', 'YOU_API_KEY' );
define( 'API_ACCESS_KEY', 'AIzaSyDzVDnxirZW7WGRkDQRsRtb1cMutbXepVY' );

//$registrationIds = array("YOUR_DEVICE_REGISTRATION_ID_HERE");
$registrationIds = array("APA91bEd6fAL-Zatz0wmcZLWsZ3FFaYS8Cc-0rtxQkFrQJ8qG7J2vI1SxAEJO_jgKVJWKMzUSvd7ILSzrOuI5SgMAGgn4D9AEITd_kJR7KkApYdEB0JEwYHpwSV0q-21N5DBdlmyIUfkEX45sDSKJhxbDrR5-Ca2Sg");

// prep the bundle
$msg = array
(
    'title'         => 'This is a title. title',
    'subtitle'      => 'This is a subtitle. subtitle',
    'alert'         => 'here is a message. message',
    'json'          => '{"big_picture_url": "http://www.hd-tecnologia.com/imagenes/articulos/2015/03/Unity-5.jpg",
                        "string_extra": "100000583627394",
                        "string_value": "value",
                        "string_key": "key",
                        "is_public": true,
                        "item_type_id": 4,
                        "numeric_extra": 0}'
);

$fields = array
(
    'registration_ids'  => $registrationIds,
    'data'              => $msg
);

$headers = array
(
    'Authorization: key=' . API_ACCESS_KEY,
    'Content-Type: application/json'
);

$ch = curl_init();
curl_setopt( $ch,CURLOPT_URL, 'https://android.googleapis.com/gcm/send' );
curl_setopt( $ch,CURLOPT_POST, true );
curl_setopt( $ch,CURLOPT_HTTPHEADER, $headers );
curl_setopt( $ch,CURLOPT_RETURNTRANSFER, true );
curl_setopt( $ch,CURLOPT_SSL_VERIFYPEER, false );
curl_setopt( $ch,CURLOPT_POSTFIELDS, json_encode( $fields ) );
$result = curl_exec($ch );
curl_close( $ch );

echo $result;
