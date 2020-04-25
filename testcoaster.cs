$RC.delete ();
$RC = Rollercoaster_Create ("0 -15 1", 2);
$RC.pushNode ("0 -14 1 " @ eulerToAxis ("30 0 0"), 2);
$RC.pushNode ("0 3 15 " @ eulerToAxis ("30 0 0"), 2);
$RC.pushNode ("0 4 15 " @ eulerToAxis ("0 0 0"), 2);
$RC.pushNode ("0 15 15 " @ eulerToAxis ("0 0 0"), 2);
$RC.pushNode ("0 16 14.5 " @ eulerToAxis ("-10 0 0"));
$RC.pushNode ("0 17 13.5 " @ eulerToAxis ("-30 0 0"));
$RC.pushNode ("0 18 12 " @ eulerToAxis ("-45 0 0")); // <--
$RC.pushNode ("0 22 4 " @ eulerToAxis ("-45 0 0"));
$RC.pushNode ("0 23 3 " @ eulerToAxis ("-30 0 0"));
$RC.pushNode ("0 24 2 " @ eulerToAxis ("-10 0 0"));
$RC.pushNode ("0 25 1.5 " @ eulerToAxis ("0 0 0"));
$RC.pushNode ("0 40 1.5 " @ eulerToAxis ("0 0 0"));
$RC.pushNode ("-1 43 2.5 " @ eulerToAxis ("10 -10 10"));
$RC.pushNode ("-3 45 4 " @ eulerToAxis ("20 -20 30"));
$RC.pushNode ("-5 46 5.5 " @ eulerToAxis ("30 -30 60"));
$RC.pushNode ("-7.5 47 7 " @ eulerToAxis ("30 -30 90"));
$RC.pushNode ("-10 47 7 " @ eulerToAxis ("20 -20 110"));
$RC.pushNode ("-12.5 46 5.5 " @ eulerToAxis ("10 -10 130"));
$RC.pushNode ("-14.5 45 4 " @ eulerToAxis ("-10 -10 150"));
$RC.pushNode ("-16.5 43 2.5 " @ eulerToAxis ("-30 -30 180"));
$RC.pushNode ("-17.5 40 1.5 " @ eulerToAxis ("0 0 180"));
$RC.pushNode ("-17.5 25 1.5 " @ eulerToAxis ("0 0 180"));
$RC.pushNode ("-17.5 24 2 " @ eulerToAxis ("10 0 180"));
$RC.pushNode ("-17.5 22 3.5 " @ eulerToAxis ("30 0 180"));
$RC.pushNode ("-17.5 21 4 " @ eulerToAxis ("10 0 180"));
$RC.pushNode ("-17.5 20 4 " @ eulerToAxis ("0 0 180"));
$RC.pushNode ("-17.5 10 4 " @ eulerToAxis ("0 0 180"));
$RC.pushNode ("-17.5 9 4 " @ eulerToAxis ("-10 0 180"));
$RC.pushNode ("-17.5 8 3.5 " @ eulerToAxis ("-30 0 180"));
$RC.pushNode ("-17.5 6 2 " @ eulerToAxis ("-10 0 180"));
$RC.pushNode ("-17.5 4 1 " @ eulerToAxis ("0 0 180"));
$RC.pushNode ("-17.5 -10 1 " @ eulerToAxis ("0 0 180"));
$RCCam = $RC.createCamera ();
// c(el).setControlObject ($RCCam.pathCam);
// $RCCam.pathCam.schedule (3000, setstate, forward);