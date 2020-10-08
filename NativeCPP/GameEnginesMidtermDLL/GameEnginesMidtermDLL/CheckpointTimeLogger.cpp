#include "CheckpointTimeLogger.h"

void CheckpointTimeLogger::ResetLogger()
{
	m_RTBC.clear();
	m_TRT = 0.0f;
}

void CheckpointTimeLogger::SaveCheckpointTime(float RTBC)
{
	m_RTBC.push_back(RTBC);

	m_TRT += RTBC;
}

float CheckpointTimeLogger::GetTotalTime()
{
	return m_TRT;
}

float CheckpointTimeLogger::GetCheckpointTime(int index)
{
	return m_RTBC[index];
}

int CheckpointTimeLogger::GetNumCheckpoints()
{
	return m_RTBC.size();
}
