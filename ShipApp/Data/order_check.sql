SELECT 
    order_line_id,
    ship_name,
    quantity,
    clean_measurement_name,
    clean_item_name,
    case_weight,
	quantity_checked,
    checked,
    notes
FROM order_record AS o
JOIN ship AS s ON o.order_ship_id = s.ship_id
JOIN measurement AS meas ON o.order_measurement_id = meas.measurement_id
JOIN item AS nm ON o.order_item_id = nm.item_id
WHERE s.ship_id = 14
ORDER BY clean_item_name;