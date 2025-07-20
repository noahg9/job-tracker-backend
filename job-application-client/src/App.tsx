import { useEffect, useState, FormEvent } from "react";
import { getAllApplications, addApplication, JobApplication } from "./api";

function App() {
    const [apps, setApps] = useState<JobApplication[]>([]);
    const [company, setCompany] = useState("");
    const [role, setRole] = useState("");

    useEffect(() => {
        getAllApplications().then(setApps);
    }, []);

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();

        const newApp: JobApplication = {
            company,
            role,
            status: 0,
            appliedDate: new Date().toISOString(),
            notes: "",
        };

        const saved = await addApplication(newApp);
        setApps([...apps, saved]);
        setCompany("");
        setRole("");
    };

    return (
        <div style={{ padding: "2rem", maxWidth: "600px", margin: "0 auto" }}>
            <h1>Job Applications</h1>
            <form onSubmit={handleSubmit}>
                <input
                    placeholder="Company"
                    value={company}
                    onChange={(e) => setCompany(e.target.value)}
                    required
                />
                <input
                    placeholder="Role"
                    value={role}
                    onChange={(e) => setRole(e.target.value)}
                    required
                />
                <button type="submit">Add</button>
            </form>

            <ul>
                {apps.map((app) => (
                    <li key={app.id}>
                        {app.company} — {app.role}
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default App;
