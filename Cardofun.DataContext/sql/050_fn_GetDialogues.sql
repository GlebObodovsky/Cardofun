CREATE FUNCTION fn_GetDialogues(@IsUnreadOnly BIT, @UserId INT)
RETURNS TABLE
AS 
RETURN
	(WITH msgCte AS
	(
		SELECT
			ROW_NUMBER() OVER 
				(
					PARTITION BY 
						IIF(SenderId <= RecipientId, SenderId, RecipientId), 
						IIF(SenderId > RecipientId, SenderId, RecipientId) 
					ORDER BY SentAt DESC
				) AS rn,
			Messages.*
		FROM 
			Messages
		WHERE
			-- if we're retrieving all messages by user id - we don't care whether it's a sender or a recipient
			(@IsUnreadOnly = 0 AND (@UserId = SenderId OR @UserId = RecipientId))
			OR
			-- if we're retrieving only unread messages by user id - we need only those where our user is recipient
			(@IsUnreadOnly = 1 AND ReadAt IS NULL AND @UserId = RecipientId)
	)
	SELECT 
		Id,
		SenderId,
		-- Sender info here
		RecipientId,
		-- Recipient info here
		Text,
		PhotoId,
		--Photo here,
		ReadAt,
		SentAt,
		SenderDeleted,
		RecipientDeleted
	FROM 
		msgCte
	WHERE 
		rn = 1)