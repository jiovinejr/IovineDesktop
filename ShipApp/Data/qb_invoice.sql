SELECT 
    order_line_id,
    ship_name,
    quantity,
	original_measurement_name,
    original_item_name,
	qb_item_name,
	qb_item_description,
    case_weight
FROM order_record AS o
JOIN ship AS s ON o.order_ship_id = s.ship_id
JOIN measurement AS meas ON o.order_measurement_id = meas.measurement_id
JOIN item AS nm ON o.order_item_id = nm.item_id
JOIN qb_item AS qb ON nm.item_qb_id = qb.id
JOIN inventory_item AS inv ON nm.item_inventory_id = inv.id
WHERE s.ship_id = 14
ORDER BY order_line_id;
