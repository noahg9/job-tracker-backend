const API_BASE = import.meta.env.VITE_API_BASE_URL;

export interface JobApplication {
    id?: number;
    company: string;
    role: string;
    status: number;
    appliedDate: string;
    round?: number;
    notes: string;
}

export async function getAllApplications(): Promise<JobApplication[]> {
    const res = await fetch(`${API_BASE}/JobApplications`);
    if (!res.ok) throw new Error("Failed to fetch job applications");
    return await res.json();
}

export async function addApplication(app: JobApplication): Promise<JobApplication> {
    const res = await fetch(`${API_BASE}/JobApplications`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(app),
    });
    if (!res.ok) throw new Error("Failed to add job application");
    return await res.json();
}

// DELETE a job application by id
export async function deleteApplication(id: number): Promise<void> {
    const res = await fetch(`${API_BASE}/JobApplications/${id}`, {
        method: "DELETE",
    });
    if (!res.ok) throw new Error("Failed to delete job application");
}

// UPDATE a job application by id
export async function updateApplication(id: number, app: JobApplication): Promise<void> {
    const res = await fetch(`${API_BASE}/JobApplications/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(app),
    });
    if (!res.ok) throw new Error("Failed to update job application");
}
